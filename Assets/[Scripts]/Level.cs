using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    int level = 1;
    int experience = 0;
    [SerializeField] ExperienceBar experienceBar;
    [SerializeField] UpgradeManager upgradePanel;
    [SerializeField] List<UpgradeData> upgrades;
    List<UpgradeData> selectedUpgrades;
    [SerializeField] List<UpgradeData> acquiredUpgrades;
    WeaponManager weaponManager;
    PassiveItems passiveItems;
    [SerializeField] List<UpgradeData> upgradeAvailableOnStart;


    private void Awake()
    {
        weaponManager = GetComponent<WeaponManager>();
        passiveItems = GetComponent<PassiveItems>();
    }

    int TO_LEVEL_UP
    {
        get
        {
            //return level * 1000;
            //return 500 + (level - 1) * 250; // 2 min 
            return 600 + (level - 1) * 200; // 10 min
        }
    }

    private void Start()
    {
        experienceBar.UpdateExperienceSlider(experience, TO_LEVEL_UP);
        experienceBar.SetLevelText(level);
        AddUpgradeIntoTheListOfAvailableUpgrades(upgradeAvailableOnStart);
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        CheckLevelUp();
        experienceBar.UpdateExperienceSlider(experience, TO_LEVEL_UP);
    }

    public void Upgrade(int selectedUpgradeID)
    {
        UpgradeData upgradeData = selectedUpgrades[selectedUpgradeID];

        if (acquiredUpgrades == null) {acquiredUpgrades = new List<UpgradeData>();}

        switch (upgradeData.upgradeType)
        {
            case UpgradeType.WeaponUpgrade:
                weaponManager.UpgradeWeapon(upgradeData);
                break;
            case UpgradeType.ItemUpgrade:
                passiveItems.UpgradeItem(upgradeData);
                break;
            case UpgradeType.GetWeapon:
                weaponManager.AddWeapon(upgradeData.weaponData);
                break;
            case UpgradeType.GetItem:
                passiveItems.Equip(upgradeData.item);
                AddUpgradeIntoTheListOfAvailableUpgrades(upgradeData.item.upgrades);
                break;
            default:
                break;
        }

        acquiredUpgrades.Add(upgradeData);
        upgrades.Remove(upgradeData);
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
        if (selectedUpgrades == null) {selectedUpgrades = new List<UpgradeData>(); }
        selectedUpgrades.Clear();
        selectedUpgrades.AddRange(GetUpgrades(4));

        //upgradePanel.OpenPanel(GetUpgrades(4));
        upgradePanel.OpenPanel(selectedUpgrades);
        experience -= TO_LEVEL_UP;
        level += 1;
        experienceBar.SetLevelText(level);
    }

    // Old method
    //public List<UpgradeData> GetUpgrades(int count)
    //{
    //    List<UpgradeData> upgradeList = new List<UpgradeData>();

    //    if (count > upgrades.Count)
    //    {
    //        count = upgrades.Count;
    //    }

    //    for (int i = 0; i < count; i++)
    //    {
    //        upgradeList.Add(upgrades[Random.Range(0, upgrades.Count)]);
    //    }

    //    return upgradeList;
    //}

    // New Method
    public List<UpgradeData> GetUpgrades(int count)
    {
        List<UpgradeData> upgradeList = new List<UpgradeData>();
        bool unlockAlreadyAdded = false;
        int safety = 0;

        while (upgradeList.Count < count && safety < 100)
        {
            safety++;

            UpgradeData randomUpgrade = upgrades[Random.Range(0, upgrades.Count)];

            bool isUnlock =
                randomUpgrade.upgradeType == UpgradeType.GetWeapon ||
                randomUpgrade.upgradeType == UpgradeType.GetItem;

            if (isUnlock && unlockAlreadyAdded)
            {
                continue;
            }

            if (isUnlock)
            {
                unlockAlreadyAdded = true;
            }

            upgradeList.Add(randomUpgrade);
        }

        return upgradeList;
    }

    internal void AddUpgradeIntoTheListOfAvailableUpgrades(List<UpgradeData> upgradesToAdd)
    {
        if (upgradesToAdd == null) { return; }

        this.upgrades.AddRange(upgradesToAdd);
    }
}
