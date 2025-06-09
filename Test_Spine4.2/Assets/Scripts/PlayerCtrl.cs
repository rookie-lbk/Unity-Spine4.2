using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Spine;
using Spine.Unity;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public partial class PlayerCtrl : MonoBehaviour
{
    private SkeletonMecanim skeletonMecanim;
    private Skeleton skeleton;

    #region 移动控制
    [SerializeField]
    private Animator animator;
    private AIPath aiPath;

    [SerializeField]
    private Text infoText;

    [Header("Movement Detection")]
    public float movementTolerance = 0.01f; // 移动检测的误差值

    private Vector2Int currentGridPosition = Vector2Int.zero; // 当前网格位置
    private Vector3 lastPosition; // 上一帧的位置
    #endregion

    #region 换装控制

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        aiPath = GetComponent<AIPath>();
        skeletonMecanim = animator.GetComponent<SkeletonMecanim>();
        skeleton = skeletonMecanim.Skeleton;
        lastPosition = transform.position;
        currentGridPosition = Vector2Int.zero;
        ChangeSkin();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = aiPath.remainingDistance - aiPath.endReachedDistance;
        if (infoText != null)
        {
            var pos = transform.position;
            infoText.text = $"pos: {pos.x}, {pos.y}, {pos.z}\n" + $"distance: {distance}";
        }

        // 使用误差值检测位置变化
        if (distance > movementTolerance)
        {
            Vector2Int newGridPosition = GetGridPositionFromTransform();
            animator.SetBool("Walking", true);
            if (newGridPosition != currentGridPosition)
            {
                currentGridPosition = newGridPosition;
                OnGridPositionChanged(currentGridPosition);
            }
            lastPosition = transform.position;
        }
        else
        {
            // currentGridPosition = Vector2Int.zero;
            animator.SetBool("Walking", false);
        }
    }

    /// <summary>
    /// 根据Transform位置计算网格坐标
    /// </summary>
    /// <returns>网格坐标 (x, y)</returns>
    public Vector2Int GetGridPositionFromTransform()
    {
        var pos = transform.position;
        if (Vector3.Distance(pos, lastPosition) < 0.01f)
        {
            return currentGridPosition;
        }

        // 将世界坐标转换为网格坐标
        float distanceX = pos.x - lastPosition.x;
        float distanceY = pos.y - lastPosition.y;
        int x = math.abs(distanceY / distanceX) > 2 ? 0 : NormalizeToUnitWithThreshold(distanceX);
        int y = math.abs(distanceX / distanceY) > 2 ? 0 : NormalizeToUnitWithThreshold(distanceY);

        // Debug.Log($"distance: ({pos.x}, {pos.y}) ({distanceX}, {distanceY})");

        return new Vector2Int(x, y);
    }

    private int NormalizeToUnitWithThreshold(float value, float threshold = 0.005f)
    {
        if (Mathf.Abs(value) < threshold)
            return 0;
        return value > 0 ? 1 : -1;
    }

    /// <summary>
    /// 网格位置改变时的回调
    /// </summary>
    /// <param name="newGridPosition">新的网格位置</param>
    private void OnGridPositionChanged(Vector2Int newGridPosition)
    {
        // Debug.Log($"Grid Position Changed: ({newGridPosition.x}, {newGridPosition.y})");

        // 在这里可以添加其他逻辑，比如触发事件、更新UI等
        animator.SetFloat("X", newGridPosition.x);
        animator.SetFloat("Y", newGridPosition.y);

        // 检查是否需要翻转动画（右、左上、右下）
        HandleSpineFlip(newGridPosition);
    }

    /// <summary>
    /// 处理Spine动画的翻转
    /// </summary>
    /// <param name="direction">移动方向</param>
    private void HandleSpineFlip(Vector2Int direction)
    {
        if (skeletonMecanim == null) return;

        bool shouldFlip = false;

        // 检查需要翻转的方向：右(1,0)、左上(-1,1)、右下(1,-1)
        if (direction.x == 1 && direction.y == 0)      // 右
        {
            shouldFlip = true;
        }
        else if (direction.x == 1 && direction.y == 1) // 左上
        {
            shouldFlip = true;
        }
        else if (direction.x == 1 && direction.y == -1) // 左下
        {
            shouldFlip = true;
        }

        // 设置Spine的X轴翻转
        skeletonMecanim.Skeleton.ScaleX = shouldFlip ? -1 : 1;
    }
}
