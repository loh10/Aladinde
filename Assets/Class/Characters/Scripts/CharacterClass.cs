using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterClass", menuName = "Scriptable Objects/Character Class")]
public class CharacterClass : ScriptableObject
{
    public string className;
    public float maxHealth;
    public float maxShield;
    public float speed;
    public float damageBonus;

    public Ability[] abilities;

    public string classDescription;
    public string firstAbilityDescription;
    public string secondAbilityDescription;
    //public string life;
    //public string damages;

}
