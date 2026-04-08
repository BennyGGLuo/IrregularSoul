using System;
using UnityEngine;

[Serializable]
public class EnemyStats
{
    public int hp = 999;
    public int damage = 1;
    public int experience_reward = 400;
    public float movementSpeed = 1f;

    public EnemyStats() { }

    public EnemyStats(EnemyStats other)
    {
        hp = other.hp;
        damage = other.damage;
        experience_reward = other.experience_reward;
        movementSpeed = other.movementSpeed;
    }

    internal void ApplyProgress(float progress)
    {
        this.hp = (int)(hp * progress);
        this.damage = (int)(damage * progress);

        // Scale exp 
        float expProgress = 1f + (progress - 1f) * 0.5f;
        experience_reward = Mathf.RoundToInt(experience_reward * expProgress);
    }
}



public class Enemy : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Avoidance")]
    [SerializeField] private float obstacleCheckDistance = 0.6f;
    [SerializeField] private float avoidanceAngle = 35f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 0.75f;
    private float nextAttackTime = 0f;

    Transform targetDestination;
    GameObject targetGameObject;
    Rigidbody2D rb;
    Character targetCharacter;

    //public EnemyStats stats;
    [SerializeField] EnemyStats stats = new EnemyStats();
    public EnemyStats Stats => stats;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (targetDestination == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 moveDirection = GetMoveDirection();
        rb.linearVelocity = moveDirection * stats.movementSpeed;
    }

    private Vector2 GetMoveDirection()
    {
        Vector2 toPlayer = (targetDestination.position - transform.position).normalized;

        // If direct path is clear, move normally
        if (!IsBlocked(toPlayer))
        {
            return toPlayer;
        }

        // Try left
        Vector2 leftDir = RotateVector(toPlayer, avoidanceAngle);
        if (!IsBlocked(leftDir))
        {
            return leftDir;
        }

        // Try right
        Vector2 rightDir = RotateVector(toPlayer, -avoidanceAngle);
        if (!IsBlocked(rightDir))
        {
            return rightDir;
        }

        // If all tested paths are blocked, stop or keep pushing lightly
        return Vector2.zero;
    }

    private bool IsBlocked(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, obstacleCheckDistance, obstacleLayer);
        return hit.collider != null;
    }

    private Vector2 RotateVector(Vector2 direction, float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        return new Vector2(
            direction.x * cos - direction.y * sin,
            direction.x * sin + direction.y * cos
        ).normalized;
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
        if (Time.time < nextAttackTime)
            return;

        if (targetCharacter == null)
        {
            targetCharacter = targetGameObject.GetComponent<Character>();
        }

        targetCharacter.TakeDamage(stats.damage);
        nextAttackTime = Time.time + attackCooldown;
    }

    public void TakeDamage(int damage)
    {
        stats.hp -= damage;

        if (stats.hp < 1)
        {
            targetGameObject.GetComponent<Level>().AddExperience(stats.experience_reward);
            GetComponent<DropOnDestroy>().CheckDrop();
            Destroy(gameObject);
        }
    }

    public void SetTarget(GameObject target)
    {
        targetGameObject = target;
        targetDestination = target.transform;
    }

    public void SetStats(EnemyStats newStats)
    {
        stats = new EnemyStats(newStats);
    }

    internal void UpdateStatsForProgress(float progress)
    {
        stats.ApplyProgress(progress);
    }
}
