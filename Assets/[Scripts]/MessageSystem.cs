using UnityEngine;

public class MessageSystem : MonoBehaviour
{
    public static MessageSystem instance;
    [SerializeField] GameObject damageMessage;

    private void Awake()
    {
        instance = this;
    }

    public void PostMessage(string text, Vector3 worldPosition)
    {
        GameObject go = Instantiate(damageMessage, transform);
        go.transform.position = worldPosition;
        go.GetComponent<TMPro.TextMeshPro>().text = text;
    }
}
