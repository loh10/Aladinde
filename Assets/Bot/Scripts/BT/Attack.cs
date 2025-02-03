using System.Collections;
using UnityEngine;

public class Attack : Node
{
    private GameObject _player;
    private LifeManager _playerLifeManager;
    private float _damageAmount;
    private float _attackInterval;
    private bool _isAttacking = false;

    public Attack(GameObject player, float damage, float interval)
    {
        _player = player;
        _damageAmount = damage;
        _attackInterval = interval;
        _playerLifeManager = player.GetComponent<LifeManager>();
    }

    public override NodeState Evaluate()
    {
        if (!_isAttacking)
        {
            _isAttacking = true;
            _player.GetComponent<MonoBehaviour>().StartCoroutine(InflictDamage());
        }

        _nodeState = NodeState.RUNNING;
        return _nodeState;
    }

    private IEnumerator InflictDamage()
    {
        while (true)
        {
            if (_playerLifeManager != null)
            {
                Debug.Log("attack");
                _playerLifeManager.TakeDamage(_damageAmount);
            }
            yield return new WaitForSeconds(_attackInterval);
        }
    }
}
