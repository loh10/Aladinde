using System;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCooldown : MonoBehaviour
{
    public Slider basicAttackSlider;
    public Slider ultimateSlider;
    private PlayerInfos _playerInfos;

    private void Start()
    {
        _playerInfos = GetComponentInParent<PlayerInfos>();
        basicAttackSlider.maxValue = _playerInfos.characterClass.abilities[1].cooldown;
        ultimateSlider.maxValue = _playerInfos.characterClass.abilities[2].maxCharge;
    }

    public void UpdateBasicAttackCooldown(float currentTime)
    {
        basicAttackSlider.value = currentTime ;
    }

    public void UpdateUltimateCooldown(float currentCharge)
    {
        ultimateSlider.value = currentCharge;
    }
}
