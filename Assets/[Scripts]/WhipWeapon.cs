using System.Collections;
using UnityEditor;
using UnityEngine;

public class WhipWeapon : WeaponBase
{
    [SerializeField] GameObject leftWhipObject;
    [SerializeField] GameObject rightWhipObject;

    PlayerMovement playerMovement;

    [SerializeField] Vector2 attackSize = new Vector2(4f, 2f);

    private void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void ApplyDamage(Collider2D[] colliders)
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

    public override void Attack()
    {
        StartCoroutine(AttackProcess());
    }

    IEnumerator AttackProcess()
    {
        bool facingRight = playerMovement.lastHorizontalVector > 0;

        if (facingRight)
            rightWhipObject.SetActive(true);
        else
            leftWhipObject.SetActive(true);

        Transform centerT = facingRight ? rightWhipObject.transform : leftWhipObject.transform;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(centerT.position, attackSize, 0f);
        ApplyDamage(colliders);

        yield return new WaitForSeconds(0.3f);
    }

    private void OnDrawGizmosSelected()
    {
        // In edit mode, playerMovement might be null, so fall back to left.
        bool facingRight = false;
        if (playerMovement != null)
            facingRight = playerMovement.lastHorizontalVector > 0;

        Transform centerT = facingRight ? rightWhipObject?.transform : leftWhipObject?.transform;
        if (centerT == null) return;

        Vector3 center = centerT.position;

        Gizmos.color = new Color(1f, 0f, 0f, 0.25f); // translucent fill
        Gizmos.DrawCube(center, attackSize);

        Gizmos.color = Color.red; // outline
        Gizmos.DrawWireCube(center, attackSize);

        // Optional: label size in Scene view
#if UNITY_EDITOR
        Handles.Label(center + Vector3.up * 0.5f, $"Whip hitbox: {attackSize}");
#endif
    }
}
