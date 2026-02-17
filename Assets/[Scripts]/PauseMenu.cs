using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject panel;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            panel.SetActive(true);
        }
    }
}
