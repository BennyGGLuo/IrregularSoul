using System;
using UnityEngine;

public class StageEeventManager : MonoBehaviour
{
    [SerializeField] StageData stageData;
    [SerializeField] EnemyManager enemyManager;
    StageTime stageTime;
    int eventIndexer;
    PlayerWinManager playerWin;

    private void Awake()
    {
        stageTime = GetComponent<StageTime>();
    }

    [Obsolete]
    private void Start()
    {
        playerWin = FindObjectOfType<PlayerWinManager>();
    }

    [Obsolete]
    private void Update()
    {
        //if (eventIndexer >= stageData.stageEvents.Count) { return; }
        //if (stageTime.time > stageData.stageEvents[eventIndexer].time) { }
        if (stageData == null) return;
        if (eventIndexer >= stageData.stageEvents.Count) return;

        StageEvent e = stageData.stageEvents[eventIndexer];
        if (stageTime.time > e.time)
        {
            switch (e.eventType)
            {
                case StageEventType.SpawnEnemy:
                    //for (int i = 0; i < e.count; i++)
                    //    enemyManager.SpawnEnemy(e.enemyToSpawn, false);
                    SpawnEnemy(e, false);
                    break;

                case StageEventType.SpawnObject:
                    //for (int i = 0; i < e.count; i++)
                    //    SpawnObject(e.objectToSpawn);
                    SpawnObject(e);
                    break;

                case StageEventType.WinStage:
                    WimStage();
                    break;
                case StageEventType.SpawnBoss:
                    SpawnEnemy(e, true);
                    break;
                default:
                    break;
            }

            Debug.Log(e.message);
            eventIndexer += 1;
        }
    }

    //private void SpawnBoss()
    //{
    //    SpawnEnemy(true);
    //}

    [Obsolete]
    private void WimStage()
    {
        playerWin.Win();
    }

    private void SpawnEnemy(StageEvent e, bool bossEnemy)
    {
        for (int i = 0; i < e.count; i++)
        {
            enemyManager.SpawnEnemy(e.enemyToSpawn, bossEnemy);
        }
    }

    private void SpawnObject(StageEvent e)
    {
        if (e.objectToSpawn == null)
        {
            Debug.LogWarning("SpawnObject event has no prefab assigned.");
            return;
        }
        if (SpawnManager.instance == null)
        {
            Debug.LogError("No SpawnManager.instance found in scene.");
            return;
        }

        for (int i = 0; i < e.count; i++)
        {
            Vector3 playerPos = GameManager.instance.playerTransform.position;
            Vector3 positionToSpawn = playerPos + UtilityTools.RandomPointInRing(10f, 20f);

            SpawnManager.instance.SpawnObject(positionToSpawn, e.objectToSpawn);
        }
    }
}
