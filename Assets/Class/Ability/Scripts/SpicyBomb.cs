using UnityEngine;

[CreateAssetMenu(fileName = "SpicyBombAbility", menuName = "Game/Abilities/Spicy Bomb")]
public class SpicyBomb : Ability
{
    public override void Activate(GameObject user)
    {
        Debug.Log(abilityName + " activated");       
        
    }
}
