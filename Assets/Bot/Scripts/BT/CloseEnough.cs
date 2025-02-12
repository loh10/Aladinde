using UnityEngine;
using UnityEngine.AI;

public class CloseEnough : Node
{
    private NavMeshAgent _agent;
    private GameObject _player;
    private float _closeRange;

    public CloseEnough(NavMeshAgent agent, GameObject player, float closeRange)
    {
        _agent = agent;
        _player = player;
        _closeRange = closeRange;
    }

    private bool _hasStopped = false;

    public override NodeState Evaluate()
    {
        float distance = Vector3.Distance(_agent.transform.position, _player.transform.position);

        if (distance < _closeRange)
        {
            if (!_hasStopped)
            {
                _agent.isStopped = true;
                _hasStopped = true;
                _nodeState = NodeState.SUCCESS;
            }
        }
        else
        {
            _agent.isStopped = false;
            _hasStopped = false;
            _nodeState = NodeState.FAILURE;
        }

        return _nodeState;
    }
}
