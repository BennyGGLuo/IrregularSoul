using UnityEngine;

public class StageProgress : MonoBehaviour
{
    StageTime stageTime;
    [SerializeField] float progressTimeRate = 30f;
    [SerializeField] float progressPerSplit = 0.2f;

    private void Awake()
    {
        stageTime = GetComponent<StageTime>();
    }

    public float Progress
    {
        get
        {
            return 1f + stageTime.time / progressTimeRate * progressPerSplit;
        }
    }
}
