using System;
using Unity.Netcode;
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
    public Vector2 mousePos;
    public Vector2 spawnTransform;

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
            IncreaseUltimateCharge(user);
        }
    }

    private void IncreaseUltimateCharge(GameObject user)
    {
        PlayerInfos playerInfos = user.GetComponent<PlayerInfos>();
        Ability ultimateAbility = playerInfos.characterClass.abilities[playerInfos.characterClass.abilities.Length - 1];
        ultimateAbility.IncreaseCharge(chargeGain);
        Debug.Log("Ultimate charge: " + ultimateAbility.GetCurrentCharge());
        if (user.GetComponentInChildren<DisplayCooldown>() != null)
        {
            user.GetComponentInChildren<DisplayCooldown>().UpdateUltimateCooldown(ultimateAbility.GetCurrentCharge());
        }

    }

    public void IncreaseCharge(float amount)
    {
        if (isChargingCapacity)
        {
            _currentCharge = Mathf.Min(maxCharge, _currentCharge + amount);
        }
    }

    public bool CanUseUltimate()
    {
        return _currentCharge >= maxCharge;
    }

    private void UseUltimate(GameObject user)
    {
        Debug.Log(abilityName + " Ultimate activated");
        if (user.GetComponentInChildren<DisplayCooldown>() != null)
        {
            user.GetComponentInChildren<DisplayCooldown>().UpdateUltimateCooldown(0);
        }
    }

    public void ResetCharge()
    {
        _currentCharge = 0f;
    }

    public float GetCurrentCharge()
    {
        return _currentCharge;
    }
}