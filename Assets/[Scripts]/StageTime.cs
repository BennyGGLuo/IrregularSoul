using UnityEngine;

public class StageTime : MonoBehaviour
{
    public float time;
    TimerUI timerUI;

    [System.Obsolete]
    private void Awake()
    {
        timerUI = FindObjectOfType<TimerUI>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        timerUI.UpdateTIme(time);
    }
}
