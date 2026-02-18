using UnityEngine;

public class Level : MonoBehaviour
{
    int level = 1;
    int experience = 0;
    [SerializeField] ExperienceBar experienceBar;
    [SerializeField] UpgradeManager upgradePanel;

    int TO_LEVEL_UP
    {
        get
        {
            return level * 1000;
        }
    }

    private void Start()
    {
        experienceBar.UpdateExperienceSlider(experience, TO_LEVEL_UP);
        experienceBar.SetLevelText(level);
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        CheckLevelUp();
        experienceBar.UpdateExperienceSlider(experience, TO_LEVEL_UP);
    }

    public void CheckLevelUp()
    {
        if (experience >= TO_LEVEL_UP)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        upgradePanel.OpenPanel();
        experience -= TO_LEVEL_UP;
        level += 1;
        experienceBar.SetLevelText(level);
    }
}
