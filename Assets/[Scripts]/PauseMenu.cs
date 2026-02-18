using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject panel;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panel.activeInHierarchy == false)
            {
                OpenMenu();
            }
            else
            {
                CloseMenu();
            }
        }
    }

    public void CloseMenu()
    {
        panel.SetActive(false);
    }

    public void OpenMenu()
    {
        panel.SetActive(true);
    }
}
