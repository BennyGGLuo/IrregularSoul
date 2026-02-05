using UnityEngine;

public class EggExpPickUpObject : MonoBehaviour, IPickUpObject
{
    [SerializeField] int amount;

    public void OnPickUp(Character character)
    {
        character.level.AddExperience(amount);
    }
}
