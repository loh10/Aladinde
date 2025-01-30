using UnityEngine;

[CreateAssetMenu(fileName = "CorrosiveSauceAbility", menuName = "Scriptable Objects/Abilities/Corrosive Sauce")]
public class CorrosiveSauce : Ability
{
    public override void Activate(GameObject user)
    {
        Debug.Log(abilityName + " activated");

    }
}
