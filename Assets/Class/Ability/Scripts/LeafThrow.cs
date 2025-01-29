using UnityEngine;

[CreateAssetMenu(fileName = "LeafThrowAbility", menuName = "Game/Abilities/Leaf Throw")]
public class LeafThrow : Ability
{
    public override void Activate(GameObject user)
    {
        Debug.Log(abilityName + " activated");

    }
}
