using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Scriptable Objects/Consumable")]
public class Consumable : ScriptableObject
{
    public string consumableName;
    public ConsumableType type;
    public float increaseHealth;
    public float increaseAbilityCharge;
    public float increaseDamageBonus;
    public float increaseSpeed;
    public float slowDuration;
    public float slowPercentageValue;

    public void ApplyEffect(PlayerInfos player, LifeManager lifeManager)
    {
        switch (type)
        {
            case ConsumableType.HealAndIncreaseHP:
                lifeManager.maxHealth += increaseHealth;
                lifeManager.currentHealth = lifeManager.maxHealth;
                break;
            case ConsumableType.IncreaseUltimateCharge:
                player.characterClass.abilities[2].IncreaseCharge(increaseAbilityCharge);
                break;
            case ConsumableType.IncreasePoopCharge:
                player.characterClass.abilities[0].IncreaseCharge(increaseAbilityCharge);
                break;
            case ConsumableType.IncreaseDamage:
                player.characterClass.damageBonus += increaseDamageBonus;
                break;
            case ConsumableType.IncreaseSpeed:
                player.characterClass.speed += increaseSpeed;
                break;
        }
    }
}

public enum ConsumableType
{
    HealAndIncreaseHP,
    IncreaseUltimateCharge,
    IncreasePoopCharge,
    IncreaseDamage,
    IncreaseSpeed,
    SlowEnemy
}
