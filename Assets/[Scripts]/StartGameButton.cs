using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public void StartGameplay()
    {
        SceneManager.LoadScene("Game Scene");
    }
}
