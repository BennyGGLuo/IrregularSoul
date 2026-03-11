using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text upgradeNameText;

    public void Set(UpgradeData upgradeData)
    {
        icon.sprite = upgradeData.icon;
        upgradeNameText.text = upgradeData.Name;
    }

    internal void Clean()
    {
        icon.sprite = null;
        upgradeNameText.text = "";
    }
}
