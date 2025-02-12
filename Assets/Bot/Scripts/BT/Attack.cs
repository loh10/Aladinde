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
    private float _lastAttackTime;
    private UnityEngine.AI.NavMeshAgent _agent;

    public Attack(GameObject player, float damage, float interval, GameObject botGameObject, Ability firstAbility)
    {
        _player = player;
        _damageAmount = damage;
        _attackInterval = interval;
        _botGameObject = botGameObject;
        _firstAbility = firstAbility;
        _agent = botGameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (_player != null)
            _playerLifeManager = _player.GetComponent<LifeManager>();
    }

    public override NodeState Evaluate()
    {
        if (_player == null || _player == _botGameObject)
            return NodeState.FAILURE;

        float distance = Vector3.Distance(_botGameObject.transform.position, _player.transform.position);

        if (distance > _firstAbility.range)
        {
            _isAttacking = false;
            _nodeState = NodeState.FAILURE;
        }
        else if (!_isAttacking && Time.time >= _lastAttackTime + _attackInterval)
        {
            _isAttacking = true;
            _lastAttackTime = Time.time;

            if (_agent != null)
                _agent.isStopped = true;

            UseAbility();
            _isAttacking = false;
            _nodeState = NodeState.SUCCESS;
        }
        else
        {
            _nodeState = NodeState.RUNNING;
        }

        return _nodeState;
    }

    private void UseAbility()
    {
        if (_firstAbility != null)
        {
            _firstAbility.Activate(_botGameObject);
            Debug.Log($"Bot {_botGameObject.name} attaque {_player.name} avec {_firstAbility.name}");

            if (_playerLifeManager != null)
            {
                _playerLifeManager.TakeDamage(_damageAmount);
            }
        }
    }
}
