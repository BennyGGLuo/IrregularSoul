using System;
using UnityEngine;

public enum DirectionalAttack
{
    None,
    Forward,
    LeftRight,
    UpDown
}

public abstract class WeaponBase : MonoBehaviour
{
    public WeaponData weaponData;
    public WeaponStats weaponStats;

    float timer;
    Character wielder;
    public Vector2 vectorOfAttack;
    [SerializeField] DirectionalAttack attackDirection;
    PlayerMovement playerMove;

    private void Awake()
    {
        
    }

    public void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            Attack();
            timer = weaponStats.timeToAttack;
        }
    }

    public void ApplyDamage(Collider2D[] colliders)
    {
        int damage = GetDamage();
        for (int i = 0; i < colliders.Length; i++)
        {
            //Debug.Log(colliders[i].gameObject.name);
            IDamageable e = colliders[i].GetComponent<IDamageable>();
            if (e != null)
            {
                PostDamage(damage, colliders[i].transform.position);
                e.TakeDamage(damage);
            }
        }
    }

    public virtual void SetData(WeaponData wd)
    {
        weaponData = wd;

        weaponStats = new WeaponStats(wd.stats.damage, wd.stats.timeToAttack, wd.stats.numberOfAttacks);
    }

    public abstract void Attack();

    public int GetDamage()
    {
        int damage = (int)(weaponData.stats.damage * wielder.damageBonus);
        return damage;
    }

    public virtual void PostDamage(int damage, Vector3 targetPosition)
    {
        MessageSystem.instance.PostMessage(damage.ToString(), targetPosition);
    }

    public void Upgrade(UpgradeData upgradeData)
    {
        weaponStats.Sum(upgradeData.weaponUpgradeStats);
    }

    public void AddOwnerCharacter(Character character)
    {
        wielder = character;
        playerMove = character.GetComponent<PlayerMovement>();
    }

    public void UpdateVectorOfAttack()
    {
        if (attackDirection == DirectionalAttack.None)
        {
            vectorOfAttack = Vector3.zero;
            return;
        }

        switch (attackDirection)
        {
            case DirectionalAttack.Forward:
                vectorOfAttack.x = playerMove.lastHorizontalCoupledVector;
                vectorOfAttack.y = playerMove.lastVerticalCoupledVector;
                break;
            case DirectionalAttack.LeftRight:
                vectorOfAttack.x = playerMove.lastHorizontalDeCoupledVector;
                vectorOfAttack.y = 0f;
                break;
            case DirectionalAttack.UpDown:
                vectorOfAttack.x = 0f;
                vectorOfAttack.y = playerMove.lastVerticalDeCoupledVector;
                break;
        }
        vectorOfAttack = vectorOfAttack.normalized;
    }
}
