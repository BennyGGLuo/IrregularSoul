using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform targetDestination;
    [SerializeField] float speed;

    GameObject targetGameObject;
    Rigidbody2D rb;

    [SerializeField] int hp = 4;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        targetGameObject = targetDestination.gameObject;
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
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp < 1)
        {
            Destroy(gameObject);
        }
    }
}
