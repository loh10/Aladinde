using UnityEngine;

[CreateAssetMenu(fileName = "LeafThrowAbility", menuName = "Scriptable Objects/Abilities/Leaf Throw")]
public class LeafThrow : Ability
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
