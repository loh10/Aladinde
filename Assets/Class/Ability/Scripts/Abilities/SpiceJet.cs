using UnityEngine;

[CreateAssetMenu(fileName = "SpiceJetAbility", menuName = "Scriptable Objects/Abilities/Spice Jet")]
public class SpiceJet : Ability
{
    public override void Activate(GameObject user)
    {
        base.Activate(user);

        Debug.Log(abilityName + " activated");
    }
}
