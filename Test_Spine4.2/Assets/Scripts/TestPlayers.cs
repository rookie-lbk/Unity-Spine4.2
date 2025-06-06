using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;

public class TestPlayers : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private Text infoText;
    private List<Transform> targetPoints = new();

    private List<GameObject> players = new();
    private float targetUpdateTimer = 0f;
    private float targetUpdateInterval;

    void Awake()
    {

        Application.targetFrameRate = 60;
    }

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            targetPoints.Add(transform.GetChild(i));
        }
        _timeLeft = _updateInterval;
    }

    public void CreatePlayer()
    {
        int count = 4;
        for (int i = 0; i < count; i++)
        {
            GameObject player = Instantiate(playerPrefab);
            int randomIndex = Random.Range(0, targetPoints.Count);
            player.transform.position = targetPoints[randomIndex].position;
            players.Add(player);
            RandomPlayerTarget(player);
        }
    }

    public void RecyclePlayer()
    {
        int count = 4;
        for (int i = 0; i < count; i++)
        {
            if (players.Count > 0)
            {
                GameObject player = players[0];
                players.RemoveAt(0);
                Destroy(player);
            }
        }
    }

    public void RecyclePlayerSkinAssets()
    {
        int count = 4;
        for (int i = 0; i < count; i++)
        {
            if (players.Count > 0)
            {
                GameObject player = players[0];
                players.RemoveAt(0);
                var playerCtrl = player.GetComponent<PlayerCtrl>();
                playerCtrl?.RecycleSkinAssets();
                Destroy(player);
            }
        }
    }
    float _updateInterval = 1f;//设定更新帧率的时间间隔为1秒  
    float _accum = .0f;//累积时间  
    int _frames = 0;//在_updateInterval时间内运行了多少帧  
    float _timeLeft;
    string fpsFormat;

    void UpdateFPS()
    {
        _timeLeft -= Time.deltaTime;
        //Time.timeScale可以控制Update 和LateUpdate 的执行速度,  
        //Time.deltaTime是以秒计算，完成最后一帧的时间  
        //相除即可得到相应的一帧所用的时间  
        _accum += Time.timeScale / Time.deltaTime;
        ++_frames;//帧数  

        if (_timeLeft <= 0)
        {
            float fps = _accum / _frames;
            //Debug.Log(_accum + "__" + _frames);  
            fpsFormat = System.String.Format("{0:F2}", fps);//保留两位小数  

            _timeLeft = _updateInterval;
            _accum = .0f;
            _frames = 0;
        }
    }

    void Update()
    {
        UpdateFPS();
        if (infoText)
        {
            infoText.text = $"Count: {players.Count}\nFPS: {fpsFormat}";
        }
        if (players.Count == 0)
        {
            return;
        }
        targetUpdateTimer += Time.deltaTime;

        if (targetUpdateTimer >= targetUpdateInterval)
        {
            RandomPlayerTarget();
            targetUpdateTimer = 0f;
            SetRandomUpdateInterval(); // 每次调用后重新设置随机间隔
        }
    }

    private void SetRandomUpdateInterval()
    {
        targetUpdateInterval = Random.Range(4f, 8f);
    }

    private void RandomPlayerTarget()
    {
        for (int i = 0; i < players.Count; i++)
        {
            RandomPlayerTarget(players[i]);
        }
    }

    private void RandomPlayerTarget(GameObject player)
    {
        var aiDestinationSetter = player.GetComponent<AIDestinationSetter>();
        if (aiDestinationSetter != null)
        {
            aiDestinationSetter.target = targetPoints[Random.Range(0, targetPoints.Count)];
        }
    }
}
