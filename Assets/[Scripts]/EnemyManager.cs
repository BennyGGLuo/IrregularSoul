using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] Vector2 spawnArea;
    [SerializeField] float spawnTimer;
    [SerializeField] GameObject player;
    [SerializeField] StageProgress stageProgress;

    List<Enemy> bossEnemiesList;
    int totalBossHealth;
    int currentBossHealth;
    [SerializeField] Slider bossHealthBar;

    private void Start()
    {
        player = GameManager.instance.playerTransform.gameObject;
        //bossHealthBar = FindAnyObjectByType<BossHPBar>(true).GetComponent<Slider>();
        BossHPBar bossBar = FindAnyObjectByType<BossHPBar>(FindObjectsInactive.Include);
        if (bossBar != null)
        {
            bossHealthBar = bossBar.GetComponent<Slider>();
        }
    }

    private void Update()
    {
        UpdateBossHealth();
    }

    private void UpdateBossHealth()
    {
        if (bossHealthBar == null) return;
        if (bossEnemiesList == null) { return; }
        if (bossEnemiesList.Count == 0) { return; }

        currentBossHealth = 0;

        for (int i = 0; i < bossEnemiesList.Count; i++)
        {
            if (bossEnemiesList[i] == null) {  continue; }
            currentBossHealth += bossEnemiesList[i].Stats.hp;
        }

        bossHealthBar.value = currentBossHealth;

        if (currentBossHealth <= 0)
        {
            bossHealthBar.gameObject.SetActive(false);
            bossEnemiesList.Clear();
            totalBossHealth = 0;
            currentBossHealth = 0;
        }
    }

    public void SpawnEnemy(EnemyData data, bool isBoss)
    {
        Vector3 pos = UtilityTools.GenerateRandomPositionSquarePattern(spawnArea) + player.transform.position;
        GameObject enemyObj = Instantiate(data.enemyPrefab, pos, Quaternion.identity, transform);
        //enemyObj.GetComponent<Enemy>().SetTarget(player);

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

        if (isBoss)
        {
            SpawnBossEnemy(enemyComp);
        }
    }

    private void SpawnBossEnemy(Enemy newBoss)
    {
        if (bossEnemiesList == null)
        {
            bossEnemiesList = new List<Enemy>();
        }

        bossEnemiesList.Add(newBoss);
        totalBossHealth += newBoss.Stats.hp;

        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(true);
            bossHealthBar.maxValue = totalBossHealth;
            bossHealthBar.value = totalBossHealth;
        }
    }
}
