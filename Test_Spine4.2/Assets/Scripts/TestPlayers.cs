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
    private Text playerCountText;
    private List<Transform> targetPoints = new();

    private List<GameObject> players = new();
    private float targetUpdateTimer = 0f;
    private float targetUpdateInterval;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            targetPoints.Add(transform.GetChild(i));
        }
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

    void Update()
    {
        if (playerCountText)
        {
            playerCountText.text = players.Count.ToString();
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
        targetUpdateInterval = Random.Range(1f, 2f);
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
