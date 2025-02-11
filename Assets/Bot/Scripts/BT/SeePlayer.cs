using UnityEngine;
using UnityEngine.AI;

public class SeePlayer : Node
{
    private NavMeshAgent _agent;
    private GameObject _player;
    private float _sightRange;

    public SeePlayer(NavMeshAgent agent, GameObject player, float sightRange)
    {
        _agent = agent;
        _player = player;
        _sightRange = sightRange;
    }

    public override NodeState Evaluate()
    {
        //distance enemy-player
        float distanceToPlayer = Vector3.Distance(_agent.transform.position, _player.transform.position);

        //check if player is in sight range
        if (distanceToPlayer <= _sightRange)
        {
            Debug.Log("See Player");
            _nodeState = NodeState.SUCCESS;
        }
        else
        {
            _nodeState = NodeState.FAILURE;
        }

        return _nodeState;
    }
}
