using UnityEngine;

public class ThrowingMagicWeapon : WeaponBase
{
    [SerializeField] GameObject magicPrefab;
    PlayerMovement playerMovement;
    [SerializeField] float spread = 0.5f;

    private void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    public override void Attack()
    {
        for (int i = 0; i < weaponStats.numberOfAttacks; i++)
        {
            GameObject throwMagic = Instantiate(magicPrefab);
            Vector3 newMagicPosition = transform.position;

            if (weaponStats.numberOfAttacks > 1)
            {
                newMagicPosition.y -= (spread * (weaponStats.numberOfAttacks-1)) / 2; // Calculate offset
                newMagicPosition.y += i * spread; // Spreading the magic along the line
            }

            throwMagic.transform.position = newMagicPosition;

            MagicProjectile throwingMagicProjectile = throwMagic.GetComponent<MagicProjectile>();
            throwingMagicProjectile.SetDirection(playerMovement.lastHorizontalVector, 0f);
            throwingMagicProjectile.damage = GetDamage();
        } 
    }
}
