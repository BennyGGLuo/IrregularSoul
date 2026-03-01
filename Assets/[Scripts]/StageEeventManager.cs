using UnityEngine;

public class StageEeventManager : MonoBehaviour
{
    [SerializeField] StageData stageData;
    [SerializeField] EnemyManager enemyManager;
    StageTime stageTime;
    int eventIndexer;

    private void Awake()
    {
        stageTime = GetComponent<StageTime>();
    }

    private void Update()
    {
        if (eventIndexer >= stageData.stageEvents.Count) { return; }

        if (stageTime.time > stageData.stageEvents[eventIndexer].time)
        {
            Debug.Log(stageData.stageEvents[eventIndexer].message);

            for (int i = 0; i < stageData.stageEvents[eventIndexer].count; i++)
            {
                //enemyManager.SpawnEnemy();
                enemyManager.SpawnEnemy(stageData.stageEvents[eventIndexer].enemyToSpawn);
            }

            eventIndexer += 1;
        }
    }
}
