using UnityEngine;

public class UltimateCharge : MonoBehaviour
{
    private float currentCharge = 0f;
    private const float maxCharge = 100f;
    private PlayerInfos _playerInfos;
    private Ability _ultimateAbility;

    private void Start()
    {
        _playerInfos = GetComponent<PlayerInfos>();
        _ultimateAbility = _playerInfos.characterClass.abilities[_playerInfos.characterClass.abilities.Length - 1];
    }

    public void IncreaseCharge(float amount)
    {
        currentCharge = Mathf.Min(maxCharge, currentCharge + amount);
    }

    public bool CanUseUltimate()
    {
        return currentCharge >= maxCharge;
    }

    public void UseUltimate(GameObject user)
    {
        if (CanUseUltimate())
        {
            _ultimateAbility.Activate(user);
            currentCharge = 0f;
        }
    }
}
