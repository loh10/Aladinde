using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Scriptable Objects/Consumable")]
public class Consumable : ScriptableObject
{
    public string consumableName;
    public ConsumableType type;
    public float increaseHealth;
    public float increaseUltimateCharge;
    public float increaseDamageBonus;
    public float increaseSpeed;
    public float slowDuration;
    public float slowPercentageValue;

    public void ApplyEffect(PlayerInfos player, LifeManager lifeManager, UltimateCharge ultimateCharge)
    {
        switch (type)
        {
            case ConsumableType.HealAndIncreaseHP:
                lifeManager.maxHealth += increaseHealth;
                lifeManager.currentHealth = lifeManager.maxHealth;
                break;
            case ConsumableType.IncreaseUltimateCharge:
                ultimateCharge.IncreaseCharge(increaseUltimateCharge);
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
    IncreaseDamage,
    IncreaseSpeed,
    SlowEnemy
}
