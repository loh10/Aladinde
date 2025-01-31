using UnityEngine;

[CreateAssetMenu(fileName = "SpicyBombAbility", menuName = "Scriptable Objects/Abilities/Spicy Bomb")]
public class SpicyBomb : Ability
{
    public override void Activate(GameObject user)
    {
        base.Activate(user);

        Debug.Log(abilityName + " activated");  
    }
}
