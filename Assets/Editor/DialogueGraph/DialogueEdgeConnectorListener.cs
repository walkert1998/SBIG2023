using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DialogueEdgeConnectorListener : IEdgeConnectorListener
{
    DialogueGraphNode node;
    DialogueOption option;

    public DialogueEdgeConnectorListener(DialogueGraphNode node)
    {
        this.node = node;
    }

    public DialogueEdgeConnectorListener(DialogueOption option)
    {
        this.option = option;
    }

    public void OnDrop(GraphView graphView, Edge edge)
    {
        if (node != null)
        {
            node.dialogueNode.destinationNodeIndex = graphView.nodes.ToList().IndexOf(edge.input.node) + 1;
            Debug.Log(graphView.nodes.ToList().IndexOf(edge.input.node));
        }
        else
        {
            option.destinationNodeIndex = graphView.nodes.ToList().IndexOf(edge.input.node);
            Debug.Log(graphView.nodes.ToList().IndexOf(edge.input.node));
        }
    }

    public void OnDropOutsidePort(Edge edge, Vector2 position)
    {
        if (node != null)
        {
            node.dialogueNode.destinationNodeIndex = -1;
        }
        else
        {
            option.destinationNodeIndex = -1;
        }
    }
}
