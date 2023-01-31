using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerNode
{
    public PlayerTree Tree { get; }

    public PlayerNode Parent { get; }
    readonly List<PlayerNode> children;

    public Vector2Int Location { get; }

    public PlayerNode(PlayerTree from, PlayerNode parent, Vector2Int location)
    {
        Parent = parent;
        children = new();
        Location = location;
        Tree = from;
    }

    public PlayerNode GetChild(int index) => children[index];
    public void AddChild(PlayerNode node) => children.Add(node);
    public void RemoveChild(PlayerNode node) => children.Remove(node);
    public int ChildrenCount => children.Count;

    public List<PlayerNode>.Enumerator GetChildrenEnumerator() => children.GetEnumerator();
}

public class PlayerTree
{
    public float score;
    public readonly AllianceColor alliance;

    public PlayerNode Root { get; private set; }

    Dictionary<Vector2Int, PlayerNode> leaves;

    public PlayerTree(Vector2Int rootLoc, float score, AllianceColor alliance)
    {
        this.score = score;
        Root = new PlayerNode(this, null, rootLoc);
        leaves = new()
        {
            { rootLoc, Root }
        };

        this.alliance = alliance;
    }

    public bool Extendable(Vector2Int loc, out Vector2Int leafLoc)
    {
        leafLoc = default;
        if (leaves.ContainsKey(loc + Vector2Int.left))
        {
            leafLoc = loc + Vector2Int.left;
            return true;
        }
        if (leaves.ContainsKey(loc + Vector2Int.right))
        {
            leafLoc = loc + Vector2Int.right;
            return true;
        }
        if (leaves.ContainsKey(loc + Vector2Int.up))
        {
            leafLoc = loc + Vector2Int.up;
            return true;
        }
        if (leaves.ContainsKey(loc + Vector2Int.up + Vector2Int.left))
        {
            leafLoc = loc + Vector2Int.up + Vector2Int.left;
            return true;
        }
        if (leaves.ContainsKey(loc + Vector2Int.up + Vector2Int.right))
        {
            leafLoc = loc + Vector2Int.up + Vector2Int.right;
            return true;
        }
        return false;
    }

    public PlayerNode Extend(PlayerNode leaf, Vector2Int to)
    {
        var newNode = new PlayerNode(this, leaf, to);
        leaf.AddChild(newNode);
        leaves.Remove(leaf.Location);
        leaves.Add(to, newNode);
        return newNode;
    }

    public void NewLeaf(PlayerNode node)
    {
        leaves[node.Location] = node;
    }

    public bool RemoveLeaf(Vector2Int loc)
    {
        return leaves.Remove(loc);
    }

    public bool IsLeaf(PlayerNode node) => node != null && leaves.ContainsKey(node.Location);
    public bool TryGetLeaf(Vector2Int loc, out PlayerNode node) => leaves.TryGetValue(loc, out node);
}
