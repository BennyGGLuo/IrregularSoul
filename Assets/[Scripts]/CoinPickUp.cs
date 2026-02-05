using UnityEngine;

public class CoinPickUp : MonoBehaviour, IPickUpObject
{
    [SerializeField] int count;

    public void OnPickUp(Character character)
    {
        character.coins.Add(count);
    }
}
