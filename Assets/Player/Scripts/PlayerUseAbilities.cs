using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUseAbilities : NetworkBehaviour
{
    [SerializeField] private PlayerInfos _playerInfos;
    private float _currentTime;
    private bool _canSimpleAttack, _canSpecialAttack;
    public GameObject _attackToSpawn;

    private void Update()
    {
        _currentTime += Time.deltaTime;
        CheckAttack(ref _canSimpleAttack, _playerInfos.characterClass.abilities[0].cooldown);
        CheckAttack(ref _canSpecialAttack, _playerInfos.characterClass.abilities[1].cooldown);
    }

    private void CheckAttack(ref bool boolAttack, float cooldown)
    {
        if (_currentTime > cooldown)
        {
            boolAttack = true;
        }
        else
        {
            boolAttack = false;
        }
    }

    public void OnSimpleAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started && _canSimpleAttack)
        {
            Debug.Log("Simple Attack");

            var ability = _playerInfos.characterClass.abilities[0];
            ability.Activate(gameObject);
            _currentTime = 0;
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started && _canSpecialAttack)
        {
            Debug.Log("Special Attack");
            _playerInfos.characterClass.abilities[1].Activate(gameObject);
            _currentTime = 0;
        }
    }

}
