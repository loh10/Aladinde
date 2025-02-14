using System;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCooldown : MonoBehaviour
{
    public Image basicAttackSlider;
    public Image ultimateSlider;
    private PlayerInfos _playerInfos;

    private void Start()
    {
        _playerInfos = GetComponentInParent<PlayerInfos>();
        basicAttackSlider.fillAmount = _playerInfos.characterClass.abilities[1].cooldown;
        ultimateSlider.fillAmount = _playerInfos.characterClass.abilities[2].maxCharge;
    }

    public void UpdateBasicAttackCooldown(float currentTime)
    {
        basicAttackSlider.fillAmount = currentTime ;
    }

    public void UpdateUltimateCooldown(float currentCharge)
    {
        ultimateSlider.fillAmount = currentCharge / _playerInfos.characterClass.abilities[2].maxCharge;

    }
}
