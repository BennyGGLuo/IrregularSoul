using UnityEngine;

public class GunWeapon : WeaponBase
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float spread = 0.5f;

    public override void Attack()
    {
        UpdateVectorOfAttack();
        for (int i = 0; i < weaponStats.numberOfAttacks; i++)
        {
            GameObject throwMagic = Instantiate(bulletPrefab);
            Vector3 newMagicPosition = transform.position;

            if (weaponStats.numberOfAttacks > 1)
            {
                newMagicPosition.y -= (spread * (weaponStats.numberOfAttacks - 1)) / 2; // Calculate offset
                newMagicPosition.y += i * spread; // Spreading the magic along the line
            }

            throwMagic.transform.position = newMagicPosition;

            MagicProjectile throwingMagicProjectile = throwMagic.GetComponent<MagicProjectile>();
            throwingMagicProjectile.SetDirection(vectorOfAttack.x, vectorOfAttack.y);
            throwingMagicProjectile.damage = GetDamage();
        }
    }
}
