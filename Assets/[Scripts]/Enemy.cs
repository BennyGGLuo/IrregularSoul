using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    Transform targetDestination;
    [SerializeField] float speed;

    GameObject targetGameObject;
    Rigidbody2D rb;

    [SerializeField] int hp = 999;
    [SerializeField] int damage = 1;

    Character targetCharacter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector3 direction = (targetDestination.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject == targetGameObject)
        {
            Attack();
        }
    }

    private void Attack()
    {
        //Debug.Log("Attacking the character!");
        if (targetCharacter == null)
        {
            targetCharacter = targetGameObject.GetComponent<Character>();
        }

        targetCharacter.TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp < 1)
        {
            Destroy(gameObject);
        }
    }

    public void SetTarget(GameObject target)
    {
        targetGameObject = target;
        targetDestination = target.transform;
    }
}
