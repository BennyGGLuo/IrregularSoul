using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int maxHP = 1000;
    public int currentHP = 1000;
    public int armor = 0;
    public float hpRegenerationRate = 1f;
    public float hpRegenerationTimer;

    [SerializeField] StatusBar hpBar;

    [HideInInspector] public Level level;
    [HideInInspector] public Coins coins;

    private GameOver gameOver;
    private bool isDead;
    [SerializeField] DataContainer dataContainer;
    public float damageBonus;

    private void Awake()
    {
        level = GetComponent<Level>();
        coins = GetComponent<Coins>();
    }

    private void Start()
    {
        ApplyPersistentUpgrades();
        hpBar.SetState(currentHP, maxHP);
    }

    private void ApplyPersistentUpgrades()
    {
        int hpUpgradeLevel = dataContainer.GetUpgradeLevel(PlayerPersistentUpgrades.HP);

        maxHP += maxHP / 10 * hpUpgradeLevel;
        //currentHP = maxHP;

        int damageUpgradeLevel = dataContainer.GetUpgradeLevel(PlayerPersistentUpgrades.Damage);

        damageBonus = 1f + 0.1f * damageUpgradeLevel;
    }

    private void Update()
    {
        hpRegenerationTimer += Time.deltaTime * hpRegenerationRate;

        if (hpRegenerationTimer > 1f)
        {
            Heal(1);
            hpRegenerationTimer -= 1f;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead == true) { return; }
        ApplyArmor(ref damage);

        currentHP -= damage;

        if (currentHP <= 0)
        {
            currentHP = 0;
            isDead = true;
            //gameOver?.TheEnd(); or 
            GetComponent<GameOver>().TheEnd();

        }

        hpBar.SetState(currentHP, maxHP);
    }

    private void ApplyArmor(ref int damage)
    {
        damage -= armor;
        if (damage < 0) { damage = 0; }
    }

    public void Heal(int amount)
    {
        if (currentHP <= 0) { return; }

        currentHP += amount;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        hpBar.SetState(currentHP, maxHP);
    }
}
