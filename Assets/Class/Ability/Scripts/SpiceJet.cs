using UnityEngine;

[CreateAssetMenu(fileName = "SpiceJetAbility", menuName = "Game/Abilities/Spice Jet")]
public class SpiceJet : Ability
{
    public override void Activate(GameObject user)
    {
        Debug.Log(abilityName + " activated");

    }
}
