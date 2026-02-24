using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] GameObject panel;
    PauseManager pauseManager;
    [SerializeField] List<UpgradeButton> upgradeButtons;

    private void Awake()
    {
        pauseManager = GetComponent<PauseManager>();
    }

    private void Start()
    {
        HideButtons();
    }

    public void OpenPanel(List<UpgradeData> upgradeDatas)
    {
        Clean();
        pauseManager.PauseGame();
        panel.SetActive(true);



        for (int i = 0; i < upgradeDatas.Count; i++)
        {
            upgradeButtons[i].gameObject.SetActive(true);
            upgradeButtons[i].Set(upgradeDatas[i]);
        }
    }

    public void ClosePanel()
    {
        HideButtons();
        pauseManager.UnPauseGame();
        panel.SetActive(false);
    }

    private void HideButtons()
    {
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            upgradeButtons[i].gameObject.SetActive(false);
        }
    }

    public void Upgrade(int pressedButtonID)
    {
        //Debug.Log("Player pressed : " + pressedButtonID.ToString());
        GameManager.instance.playerTransform.GetComponent<Level>().Upgrade(pressedButtonID);
        ClosePanel();
    }

    public void Clean()
    {
        for (int i = 0; i < upgradeButtons.Count;i++)
        {
            upgradeButtons[i].Clean();
        }
    }
}
