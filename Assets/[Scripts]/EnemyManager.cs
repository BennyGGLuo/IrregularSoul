using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] Vector2 spawnArea;
    [SerializeField] float spawnTimer;
    [SerializeField] GameObject player;
    [SerializeField] StageProgress stageProgress;
    //float timer;

    //private void Update()
    //{
    //    timer -= Time.deltaTime;
    //    if (timer < 0)
    //    {
    //        SpawnEnemy();
    //        timer = spawnTimer;
    //    }
    //}

    //public void SpawnEnemy()
    //{
    //    Vector3 position = GenerateRandomPosition();

    //    position += player.transform.position;

    //    GameObject newEnemy = Instantiate(enemy);
    //    newEnemy.transform.position = position;
    //    newEnemy.GetComponent<Enemy>().SetTarget(player);
    //    newEnemy.transform.parent = transform;
    //}
    public void SpawnEnemy(EnemyData data)
    {
        Vector3 pos = UtilityTools.GenerateRandomPositionSquarePattern(spawnArea) + player.transform.position;
        GameObject enemyObj = Instantiate(data.enemyPrefab, pos, Quaternion.identity, transform);
        enemyObj.GetComponent<Enemy>().SetTarget(player);

        Enemy enemyComp = enemyObj.GetComponent<Enemy>();
        if (enemyComp == null)
        {
            Debug.LogError($"Prefab '{data.enemyPrefab.name}' has no Enemy component.", enemyObj);
            return;
        }

        enemyComp.SetTarget(player);

        if (data.stats != null)
            enemyComp.SetStats(data.stats);

        enemyComp.UpdateStatsForProgress(stageProgress.Progress);
    }
    
}
