using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUseAbilities : MonoBehaviour
{
    private PlayerInfos _playerInfos;
    private Ability _ultimate;

    private void Start()
    {
        _playerInfos = GetComponent<PlayerInfos>();
        _ultimate = _playerInfos.characterClass.abilities[2];

        //set abilities charges to 0 else they save the charge from the previous game 
        foreach (Ability ability in _playerInfos.characterClass.abilities)
        {
            ability.ResetCharge();
        }
    }

    public void OnSimpleAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            _playerInfos.characterClass.abilities[1].Activate(gameObject);
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            if (_ultimate != null && _ultimate.CanUseUltimate())
            {
                _ultimate.Activate(gameObject);
            }
            else
            {
                Debug.Log("Ultimate not ready!");
            }
        }
    }
}
