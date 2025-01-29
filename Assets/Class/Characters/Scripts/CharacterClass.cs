using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterClass", menuName = "Game/Character Class")]
public class CharacterClass : ScriptableObject
{
    public string className;
    public float maxHealth;
    public float speed;
    public float damageBonus;

    public Ability[] abilities;
}
