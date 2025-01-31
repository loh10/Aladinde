using UnityEngine;

[CreateAssetMenu(fileName = "LeafThrowAbility", menuName = "Scriptable Objects/Abilities/Leaf Throw")]
public class LeafThrow : Ability
{
    public override void Activate(GameObject user)
    {
        base.Activate(user);

        Debug.Log(abilityName + " activated");
    }
}
