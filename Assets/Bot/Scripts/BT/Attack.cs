using System.Collections;
using UnityEngine;

public class Attack : Node
{
    private GameObject _player;
    private LifeManager _playerLifeManager;
    private float _damageAmount;
    private float _attackInterval;
    private bool _isAttacking = false;
    private Ability _firstAbility;
    private GameObject _botGameObject;

    public Attack(GameObject player, float damage, float interval, GameObject botGameObject, Ability firstAbility)
    {
        _player = player;
        _damageAmount = damage;
        _attackInterval = interval;
        _playerLifeManager = player.GetComponent<LifeManager>();
        _botGameObject = botGameObject;
        _firstAbility = firstAbility;
    }

    public override NodeState Evaluate()
    {
        if (_player != null)
        {
            float distance = Vector3.Distance(_botGameObject.transform.position, _player.transform.position);
            if (distance >= _firstAbility.range)
            {
                _isAttacking = false;
                _nodeState = NodeState.FAILURE;
            }
            else if (!_isAttacking)
            {
                _isAttacking = true;
                _botGameObject.GetComponent<MonoBehaviour>().StartCoroutine(DelayedAttack());
                _nodeState = NodeState.RUNNING;
            }
        }
        else
        {
            return NodeState.FAILURE;
        }

        return _nodeState;
    }

    private IEnumerator DelayedAttack()
    {
        yield return new WaitForSeconds(0.5f);
        while (_isAttacking)
        {
            float distance = Vector3.Distance(_botGameObject.transform.position, _player.transform.position);
            if (distance >= _firstAbility.range)
            {
                Debug.Log("Enemy out of range, stopping attack.");
                _isAttacking = false;
                yield break;
            }

            _firstAbility.Activate(_botGameObject);
            yield return new WaitForSeconds(_attackInterval);
        }
    }
}