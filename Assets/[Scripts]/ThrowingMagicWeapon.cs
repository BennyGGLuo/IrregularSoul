using UnityEngine;

public class ThrowingMagicWeapon : MonoBehaviour
{
    [SerializeField] GameObject magicPrefab;
    [SerializeField] float timeToAttack;
    float timer;
    PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void Update()
    {
        if (timer < timeToAttack)
        {
            timer += Time.deltaTime;
            return;
        }

        timer = 0;
        SpawnMagic();
    }

    private void SpawnMagic()
    {
        GameObject throwMagic = Instantiate(magicPrefab);
        throwMagic.transform.position = transform.position;
        throwMagic.GetComponent<MagicProjectile>().SetDirection(playerMovement.lastHorizontalVector, 0f);
    }
}
