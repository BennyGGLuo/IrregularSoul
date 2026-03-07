using Unity.VisualScripting;
using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    Vector3 direction;
    [SerializeField] float speed;
    public int damage = 5;

    //bool hitDetected = false;

    float ttl = 6f;


    public void SetDirection(float dir_x, float dir_y)
    {
        direction = new Vector3 (dir_x, dir_y);

        if (dir_x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = scale.x * -1;
            transform.localScale = scale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;


        if (Time.frameCount % 6 == 0)
        {
            Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, 0.3f);
            //foreach (Collider2D c in hit)
            //{
            //    IDamageable enemy = c.GetComponent<IDamageable>();
            //    if (enemy != null)
            //    {
            //        PostDamage(damage, transform.position);
            //        enemy.TakeDamage(damage);
            //        break;
            //    }
            //}
            //if (hitDetected == true)
            //{
            //    Destroy(gameObject);
            //}
            foreach (Collider2D c in hit)
            {
                if (c == null) continue;

                // Ignore self
                if (c.gameObject == gameObject) continue;

                // Ignore player
                if (c.GetComponent<PlayerMovement>() != null) continue;

                // Ignore player weapon parent objects
                if (c.GetComponentInParent<WeaponBase>() != null) continue;

                // 1. Damage enemies
                IDamageable damageable = c.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    PostDamage(damage, transform.position);
                    damageable.TakeDamage(damage);
                    Destroy(gameObject);
                    return;
                }

                // 2. Destroy if it hits obstacle layer
                if (c.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                {
                    Destroy(gameObject);
                    return;
                }

                // Anything else gets ignored
            }
        }

        ttl -= Time.deltaTime;
        if (ttl < 0f)
        {
            Destroy(gameObject);
        }
    }

    public void PostDamage(int damage, Vector3 worldPosition)
    {
        MessageSystem.instance.PostMessage(damage.ToString(), worldPosition);
    }

    private void OnDrawGizmosSelected()
    {
        // Shows the OverlapCircle radius you use for hit detection
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
