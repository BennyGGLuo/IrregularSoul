using UnityEngine;

public class Coins : MonoBehaviour
{
    public int coinAquired;

    [SerializeField] TMPro.TextMeshProUGUI coinsCountText;

    public void Add(int count)
    {
        coinAquired += count;
        coinsCountText.text = "Coins:" + coinAquired.ToString();
    }
}
