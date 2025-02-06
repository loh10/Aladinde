using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer : Node
{
    private NavMeshAgent _agent;
    private GameObject _player;

    public ChasePlayer(NavMeshAgent agent,  GameObject player)
    {
        _agent = agent;
        _player = player;
    }

    public override NodeState Evaluate()
    {
        _agent.SetDestination(_player.transform.position);

        _nodeState = NodeState.SUCCESS;
        return _nodeState;
    }
}
