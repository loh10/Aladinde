using UnityEditor.Playables;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Scriptable Objects/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName;
    public string abilityDescription;
    public float cooldown;
    public float range;
    public float damages;
    public float chargeGain;

    public bool isChargingCapacity;
    public float maxCharge;
    private float _currentCharge;

    public Sprite icon;
    public GameObject abilityPrefab;

    public virtual void Activate(GameObject user)
    {
        if (isChargingCapacity)
        {
            if (CanUseUltimate())
            {
                UseUltimate(user);
                ResetCharge();
            }
            else
            {
                Debug.Log("Ultimate not ready!");
            }
        }
        else
        {
            Debug.Log(abilityName + " triggered by " + user.name);
            IncreaseUltimateCharge(user);
        }
    }

    private void IncreaseUltimateCharge(GameObject user)
    {
        PlayerInfos playerInfos = user.GetComponent<PlayerInfos>();
        Ability ultimateAbility = playerInfos.characterClass.abilities[playerInfos.characterClass.abilities.Length - 1];

        ultimateAbility.IncreaseCharge(chargeGain);
        Debug.Log($"Ultimate {ultimateAbility.abilityName} charge increased by {chargeGain}");
    }

    public void IncreaseCharge(float amount)
    {
        if (isChargingCapacity)
        {
            _currentCharge = Mathf.Min(maxCharge, _currentCharge + amount);
            Debug.Log($"Charge {abilityName}: {_currentCharge}/{maxCharge}");
        }
    }

    public bool CanUseUltimate()
    {
        return _currentCharge >= maxCharge;
    }

    private void UseUltimate(GameObject user)
    {
        Debug.Log(abilityName + " Ultimate activated");
    }

    public void ResetCharge()
    {
        _currentCharge = 0f;
    }
}