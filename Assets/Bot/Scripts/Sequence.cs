using System.Collections.Generic;


public class Sequence : Node
{
    protected List<Node> nodes = new List<Node>();

    public Sequence(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Evaluate()
    {
        bool anyChildIsRunning = false;

        foreach (var node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    anyChildIsRunning = true;
                    break;
                case NodeState.SUCCESS:
                    break;
                case NodeState.FAILURE:
                    _nodeState = NodeState.FAILURE;
                    return _nodeState;
                default:
                    break;

            }
        }

        _nodeState = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;

        return _nodeState;
    }
}