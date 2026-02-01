using UnityEngine;

public class WhipWeapon : MonoBehaviour
{
    float timeToAttack = 4f;
    float timer;

    [SerializeField] GameObject leftWhipObject;
    [SerializeField] GameObject rightWhipObject;

    PlayerMovement playerMovement;

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
        Debug.Log("Attack");
        timer = timeToAttack;

        if (playerMovement.lastHorizontalVector > 0)
        {
            rightWhipObject.SetActive(true);
        }
        else
        {
            leftWhipObject.SetActive(true);
        }
    }
}
