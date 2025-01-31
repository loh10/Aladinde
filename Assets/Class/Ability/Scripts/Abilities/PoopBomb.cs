using UnityEngine;

[CreateAssetMenu(fileName = "PoopBombAbility", menuName = "Scriptable Objects/Abilities/Poop Bomb")]
public class PoopBomb : Ability
{
    public float slowDuration;
    public float slowPercentage;

    public override void Activate(GameObject user)
    {
        base.Activate(user);

        Debug.Log("poop bomb");
    }
}
