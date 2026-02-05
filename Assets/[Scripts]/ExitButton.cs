using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void QuitApplication()
    {
        Debug.Log("Application has ended");
        Application.Quit();
    }
}
