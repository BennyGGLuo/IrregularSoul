using UnityEngine;

public class ThrowingMagicWeapon : WeaponBase
{
    [SerializeField] GameObject magicPrefab;
    PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    public override void Attack()
    {
        GameObject throwMagic = Instantiate(magicPrefab);
        throwMagic.transform.position = transform.position;
        MagicProjectile throwingMagicProjectile = throwMagic.GetComponent<MagicProjectile>();
        throwingMagicProjectile.SetDirection(playerMovement.lastHorizontalVector, 0f);
        throwingMagicProjectile.damage = weaponStats.damage;
    }
}
