using UnityEngine;

public class GarlickWeapon : WeaponBase
{
    [SerializeField] float attackAreaSize = 3f;
    [SerializeField] GameObject garlickVisual;
    [SerializeField] SpriteRenderer garlickSpriteRenderer;

    private void Start()
    {
        SyncVisualToDamageRadius();

        if (garlickVisual != null)
        {
            garlickVisual.SetActive(false);
        }
    }

    public override void Attack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackAreaSize);
        ApplyDamage(colliders);

        if (garlickVisual != null)
        {
            garlickVisual.SetActive(false);
            garlickVisual.SetActive(true);
        }
    }

    private void SyncVisualToDamageRadius()
    {
        if (garlickVisual == null || garlickSpriteRenderer == null || garlickSpriteRenderer.sprite == null)
            return;

        // OverlapCircleAll uses radius, so visual should match diameter
        float targetDiameter = attackAreaSize * 2f;

        // Sprite bounds at scale 1, based on sprite import settings / pixels per unit
        Vector2 spriteSize = garlickSpriteRenderer.sprite.bounds.size;

        if (spriteSize.x <= 0f || spriteSize.y <= 0f)
            return;

        garlickVisual.transform.localPosition = Vector3.zero;
        garlickVisual.transform.localScale = new Vector3(
            targetDiameter / spriteSize.x,
            targetDiameter / spriteSize.y,
            1f
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackAreaSize);
    }
}
