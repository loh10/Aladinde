using UnityEngine;

[CreateAssetMenu(fileName = "PoopBombAbility", menuName = "Scriptable Objects/Abilities/Poop Bomb")]
public class PoopBomb : Ability
{
    public float slowDuration;
    public float slowPercentage;

    public override void Activate(GameObject user)
    {
        Debug.Log(abilityName + " activated by " + user.name);

        Debug.Log("Enemies slowed by " + slowPercentage * 100 + "% for " + slowDuration + " seconds.");
    }
}
