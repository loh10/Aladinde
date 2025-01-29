using UnityEngine;

[CreateAssetMenu(fileName = "GrillStrikeAbility", menuName = "Game/Abilities/Grill Strike")]
public class GrillStrike : Ability
{
    public override void Activate(GameObject user)
    {
        Debug.Log(abilityName + " activated");

    }
}
