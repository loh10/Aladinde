using UnityEngine;

[CreateAssetMenu(fileName = "SmokedShieldAbility", menuName = "Game/Abilities/Smoked Shield")]
public class SmokedShield : Ability
{
    public float duration ;

    public override void Activate(GameObject user)
    {
        LifeManager lifeManager = user.GetComponent<LifeManager>();
        if (lifeManager != null)
        {
            lifeManager.ActiShield(duration);
            Debug.Log(abilityName + " activated");
        }
    }
}
