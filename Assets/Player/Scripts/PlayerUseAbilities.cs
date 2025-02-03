using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUseAbilities : MonoBehaviour
{
    [SerializeField] private PlayerInfos _playerInfos;

    public void OnSimpleAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started)
        {
            Debug.Log("Simple Attack");
            _playerInfos.characterClass.abilities[0].Activate(gameObject);
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext ctx)
    {
        if(ctx.phase == InputActionPhase.Started)
        {
            Debug.Log("Special Attack");
            _playerInfos.characterClass.abilities[1].Activate(gameObject);
        }
    }
}
