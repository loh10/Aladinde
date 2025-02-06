using UnityEngine;
using UnityEngine.AI;

public class Patrol : Node
{
    private NavMeshAgent _agent;
    private Vector3 _center;
    private float _range;
    private float _waitTime = 2f;
    private Vector3 _targetPoint;

    public Patrol(NavMeshAgent agent, Vector3 center, float range)
    {
        _agent = agent;
        _center = center;
        _range = range;
        ChooseRandomPoint();
    }

    public override NodeState Evaluate()
    {
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            if (_waitTime > 0)
            {
                _waitTime -= Time.deltaTime;
            }
            else
            {
                ChooseRandomPoint();
                _waitTime = 0.5f;
            }
        }

        _nodeState = NodeState.SUCCESS;
        return _nodeState;
    }

    private void ChooseRandomPoint()
    {
        int attempts = 10;

        for (int i = 0; i < attempts; i++)
        {
            Vector3 randomPoint = _center + new Vector3(
                Random.Range(-_range, _range),
                Random.Range(-_range, _range),
                0
            );

            if (Vector3.Distance(_agent.transform.position, randomPoint) >= _range / 2)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
                {
                    _targetPoint = hit.position;
                    _agent.SetDestination(_targetPoint);
                    return;
                }
            }
        }

        //if no point found go there
        _targetPoint = _center + new Vector3(_range / 2, _range / 2, 0);
        _agent.SetDestination(_targetPoint);
    }
}
