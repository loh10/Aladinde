using UnityEngine;

[CreateAssetMenu(fileName = "SmokedShieldAbility", menuName = "Scriptable Objects/Abilities/Smoked Shield")]
public class SmokedShield : Ability
{
    public float duration;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        user.GetComponent<PlayerLifeManager>().ActiShield(duration);
        Debug.Log(abilityName + " activated");

    }
}
