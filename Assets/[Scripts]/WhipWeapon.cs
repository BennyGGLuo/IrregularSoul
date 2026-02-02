using UnityEditor;
using UnityEngine;

public class WhipWeapon : MonoBehaviour
{
    [SerializeField] float timeToAttack = 4f;
    float timer;

    [SerializeField] GameObject leftWhipObject;
    [SerializeField] GameObject rightWhipObject;

    PlayerMovement playerMovement;

    [SerializeField] Vector2 whipAttackSize = new Vector2(4f, 2f);
    [SerializeField] int whipDamage = 1;

    private void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0 )
        {
            Attack();
        }
    }

    private void Attack()
    {
        timer = timeToAttack;

        //if (playerMovement.lastHorizontalVector > 0)
        //{
        //    rightWhipObject.SetActive(true);
        //    Collider2D[] colliders = Physics2D.OverlapBoxAll(leftWhipObject.transform.position,
        //        whipAttackSize, 0f);
        //    ApplyDamage(colliders);
        //}
        //else
        //{
        //    leftWhipObject.SetActive(true);
        //    Collider2D[] colliders = Physics2D.OverlapBoxAll(leftWhipObject.transform.position,
        //        whipAttackSize, 0f);
        //    ApplyDamage(colliders);
        //}
        bool facingRight = playerMovement.lastHorizontalVector > 0;

        if (facingRight)
            rightWhipObject.SetActive(true);
        else
            leftWhipObject.SetActive(true);

        Transform centerT = facingRight ? rightWhipObject.transform : leftWhipObject.transform;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(centerT.position, whipAttackSize, 0f);
        ApplyDamage(colliders);
    }

    private void ApplyDamage(Collider2D[] colliders)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            //Debug.Log(colliders[i].gameObject.name);
            IDamageable e = colliders[i].GetComponent<IDamageable>();
            if (e != null)
            {
                e.TakeDamage(whipDamage);
            }
        }
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
        Gizmos.DrawCube(center, whipAttackSize);

        Gizmos.color = Color.red; // outline
        Gizmos.DrawWireCube(center, whipAttackSize);

        // Optional: label size in Scene view
#if UNITY_EDITOR
        Handles.Label(center + Vector3.up * 0.5f, $"Whip hitbox: {whipAttackSize}");
#endif
    }
}
