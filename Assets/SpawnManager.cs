using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    [System.Obsolete]
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void SpawnObject(Vector3 worldPosition, GameObject toSpawn)
    {
        //Transform t = Instantiate(toSpawn, transform).transform;
        //t.position = worldPosition;
        Instantiate(toSpawn, worldPosition, Quaternion.identity, transform);
    }
}
