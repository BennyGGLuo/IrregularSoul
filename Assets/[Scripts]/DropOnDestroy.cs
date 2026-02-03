using UnityEngine;

public class DropOnDestroy : MonoBehaviour
{
    [SerializeField] GameObject healthPickUp;

    private void OnDestroy()
    {
        Transform t = Instantiate(healthPickUp).transform;
        t.position = transform.position;
    }
}
