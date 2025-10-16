using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree
{
    private Node root;

    public BehaviorTree(Node root)
    {
        this.root = root;
    }

    public NodeState Update()
    {
        return root.Evaluate();
    }
}
// Start is called once before the first execution of Update after the MonoBehaviour is created
public enum NodeState
{
    RUNNING,
    SUCCESS,
    FAILURE
}
public abstract class Node
{
    protected NodeState state;
public NodeState currentState => state;
    
    public abstract NodeState Evaluate();
}
public class SelectorNode : Node

{
    protected List<Node> children = new List<Node>();
    public SelectorNode(List<Node> nodes)
    {
        children = nodes;
    }


    public override NodeState Evaluate()
    {
        bool anyChildRunning = false;
        foreach (var node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.SUCCESS:
                    state = NodeState.SUCCESS;
                    return state;
                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    return state;
                case NodeState.FAILURE:
                    continue;
                default:
                    continue;
            }
        }
        state = NodeState.FAILURE;
        return state;
    }
}
public class ConditionNode : Node
{
    private Func<bool> condition; // hàm điều kiện
    public ConditionNode(Func<bool> condition)
    {
        this.condition = condition;
    }

    public override NodeState Evaluate()
    {
        state = condition() ? NodeState.SUCCESS : NodeState.FAILURE;
        return state;
    }
}

public class ActionNode : Node
{
    private Func<NodeState> action; // hàm hành động
    public ActionNode(Func<NodeState> action)
    {
        this.action = action;
    }

    public override NodeState Evaluate()
    {
        state = action();
        return state;
    }
}
public class SequenceNode : Node
{
    protected List<Node> children = new List<Node>();
    public SequenceNode(List<Node> nodes)
    {
        children = nodes;
    }

    public override NodeState Evaluate()
    {
        foreach (var node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    return state;
                case NodeState.FAILURE:
                    state = NodeState.FAILURE;
                    return state;
                case NodeState.SUCCESS:
                    continue;
                default:
                    continue;
            }
        }
        state = NodeState.SUCCESS;
        return state;
    }
}