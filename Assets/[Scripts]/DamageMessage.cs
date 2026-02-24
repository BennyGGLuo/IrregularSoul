using UnityEngine;

public class DamageMessage : MonoBehaviour
{
    [SerializeField] float ttl = 2f;

    private void Update()
    {
        ttl -= Time.deltaTime;
        if (ttl < 0)
        {
            Destroy(gameObject);
        }
    }
}
