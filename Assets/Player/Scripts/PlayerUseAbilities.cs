using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUseAbilities : MonoBehaviour
{
    private PlayerInfos _playerInfos;
    private UltimateCharge _ultimateCharge;

    private void Start()
    {
        _playerInfos = GetComponent<PlayerInfos>();
        _ultimateCharge = GetComponent<UltimateCharge>();
    }

    public void OnSimpleAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            _playerInfos.characterClass.abilities[0].Activate(gameObject);
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            if (_ultimateCharge != null && _ultimateCharge.CanUseUltimate())
            {
                _ultimateCharge.UseUltimate(gameObject);
            }
            else
            {
                Debug.Log("Ultimate not ready!");
            }
        }
    }

}
