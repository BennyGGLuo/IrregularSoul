using UnityEngine;

public enum UpgradeType
{
    WeaponUpgrade,
    ItemUpgrade,
    GetWeapon,
    GetItem
}

[CreateAssetMenu]
public class UpgradeData : ScriptableObject
{
    public UpgradeType upgradeType;
    public string Name;
    [TextArea] public string Description;
    public Sprite icon;
    public WeaponData weaponData;
    public WeaponStats weaponUpgradeStats;
    public Item item;
    public ItemStats itemStats;
}
