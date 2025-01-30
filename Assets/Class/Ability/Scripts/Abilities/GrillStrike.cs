using UnityEngine;

[CreateAssetMenu(fileName = "GrillStrikeAbility", menuName = "Scriptable Objects/Abilities/Grill Strike")]
public class GrillStrike : Ability
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
