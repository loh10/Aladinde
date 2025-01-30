using UnityEngine;

[CreateAssetMenu(fileName = "SpiceJetAbility", menuName = "Scriptable Objects/Abilities/Spice Jet")]
public class SpiceJet : Ability
{
    public override void Activate(GameObject user)
    {
        Debug.Log(abilityName + " activated");

        UltimateCharge ultimateCharge = user.GetComponent<UltimateCharge>();
        if (ultimateCharge != null)
        {
            Debug.Log("utlitmate increase");
            ultimateCharge.IncreaseCharge(chargeGain);
        }
    }
}
