using UnityEngine;

public class LevelCompletion : MonoBehaviour
{
    [SerializeField] float timeToCompleteLevel;
    StageTime stageTime;
    PauseManager pauseManager;
    [SerializeField] CompleteStagePanel levelCompletePanel;

    [System.Obsolete]
    private void Awake()
    {
        stageTime = GetComponent<StageTime>();
        pauseManager = FindObjectOfType<PauseManager>();
        levelCompletePanel = FindObjectOfType<CompleteStagePanel>(true);
    }

    private void Update()
    {
        if (stageTime.time > timeToCompleteLevel)
        {
            pauseManager.PauseGame();
            levelCompletePanel.gameObject.SetActive(true);
        }
    }
}
