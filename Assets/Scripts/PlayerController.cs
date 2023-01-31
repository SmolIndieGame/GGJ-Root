using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class PlayerController
{
    const float InitialPenalty = 4;
    const float PenaltyMultiplier = 2;
    const float InitialScore = 20;

    static PlayerTree p1;
    static PlayerTree p2;

    static PlayerTree current;

    #region Fields For Handling Turns
    static PlayerNode touchingLeaf;
    static Dictionary<Vector2Int, (float changeScore, PlayerNode enemyNode)> growableLocs = new();
    static bool branching;
    static float costPenalty;
    #endregion

    public static VisualGuider visualGuider { private get; set; }

    public static event System.Action<float> p1ScoreChange;
    public static event System.Action<float> p2ScoreChange;
    public static event System.Action<bool> CanEndTurnChange;
    public static event System.Action<int> onEndTurn;

    public static void Init()
    {
        var p1Color = new AllianceColor
        {
            twig = new Color32(150, 0, 0, 255),
            leaf = new Color32(255, 0, 0, 255)
        };
        var p2Color = new AllianceColor
        {
            twig = new Color32(0, 0, 150, 255),
            leaf = new Color32(0, 0, 255, 255)
        };
        p1 = new PlayerTree(new Vector2Int(3, 0), InitialScore, p1Color);
        p2 = new PlayerTree(new Vector2Int(14, 0), InitialScore, p2Color);

        var tmpLoc = new Vector2Int(3, 0);
        BlockGrid.ExtendLeaf(tmpLoc, tmpLoc, p1.Root);
        tmpLoc = new Vector2Int(14, 0);
        BlockGrid.ExtendLeaf(tmpLoc, tmpLoc, p2.Root);

        current = p1;
        touchingLeaf = null;

        p1ScoreChange?.Invoke(p1.score);
        p2ScoreChange?.Invoke(p2.score);
    }

    public static void EndCurrentPlayerTurn()
    {
        if (costPenalty == 0)
            return;
        if (touchingLeaf != null)
            Cancel();

        // TODO: Please do not hard code.
        DominationReward(new Vector2Int(8, -17));
        DominationReward(new Vector2Int(8, -18));
        DominationReward(new Vector2Int(9, -17));
        DominationReward(new Vector2Int(9, -18));

        if (p1 == current)
            p1ScoreChange?.Invoke(current.score);
        else
            p2ScoreChange?.Invoke(current.score);

        current = current == p1 ? p2 : p1;
        costPenalty = 0;
        CanEndTurnChange?.Invoke(false);
        onEndTurn?.Invoke(current == p1 ? 1 : 2);
    }

    static void DominationReward(Vector2Int loc)
    {
        if (!IsAlly(loc))
            return;

        current.score++;
    }

    public static void Click(Vector2Int loc)
    {
        if (touchingLeaf != null && loc == touchingLeaf.Location)
        {
            Cancel();
            return;
        }

        PlayerNode node = BlockGrid.GetTreeNode(loc);
        if (current == node?.Tree && BlockGrid.GetType(loc) == BlockType.Twig)
        {
            Cancel();
            current.NewLeaf(node);
            BlockGrid.SetTwigToLeaf(node.Location);
            branching = true;

            touchingLeaf = node;
            CreateGuide(loc);
            return;
        }

        if (current.IsLeaf(node))
        {
            Cancel();
            touchingLeaf = node;
            CreateGuide(loc);
            return;
        }

        if (touchingLeaf == null || !growableLocs.TryGetValue(loc, out var changes))
            return;

        if (changes.enemyNode != null)
            DestroySubTree(changes.enemyNode);

        Vector2Int leafLoc = touchingLeaf.Location;
        BlockGrid.ExtendLeaf(leafLoc, loc, current.Extend(touchingLeaf, loc));

        current.score += changes.changeScore;
        costPenalty = costPenalty == 0 ? InitialPenalty : costPenalty * PenaltyMultiplier;

        // Add support for more than 2 players
        if (p1 == current)
            p1ScoreChange?.Invoke(current.score);
        else
            p2ScoreChange?.Invoke(current.score);
        CanEndTurnChange?.Invoke(true);

        touchingLeaf = null;
        branching = false;
        growableLocs.Clear();
        visualGuider.RemoveAllGuide();
    }

    static void DestroySubTree(PlayerNode node)
    {
        if (node == null)
            return;
        if (node.Parent != null)
        {
            if (node.Parent.ChildrenCount == 1)
            {
                node.Tree.NewLeaf(node.Parent);
                BlockGrid.SetTwigToLeaf(node.Parent.Location);
            }
            node.Parent.RemoveChild(node);
        }

        DestroyRecursive(node);
    }

    static void DestroyRecursive(PlayerNode node)
    {
        using var iter = node.GetChildrenEnumerator();
        while (iter.MoveNext())
            DestroyRecursive(iter.Current);

        node.Tree.RemoveLeaf(node.Location);
        BlockGrid.SetTreeBlockToDirt(node.Location);
    }

    public static void Cancel()
    {
        if (touchingLeaf == null)
            return;
        if (branching)
            BlockGrid.SetLeafToTwig(touchingLeaf.Location);
        touchingLeaf = null;
        branching = false;
        growableLocs.Clear();
        visualGuider.RemoveAllGuide();
    }

    public static List<Vector2Int> expandableDirection = new List<Vector2Int>()
    {
        Vector2Int.left, Vector2Int.down, Vector2Int.right, new Vector2Int(1, -1), new Vector2Int(-1, -1)
    };

    static void CreateGuide(Vector2Int loc)
    {
        visualGuider.PlaceTargetAt(loc);
        foreach (var vec in expandableDirection)
        {
            if (CanGrow(loc, loc + vec, out var changes))
            {
                growableLocs[loc + vec] = changes;
                visualGuider.PlaceGuideAt(loc + vec, changes.changeScore);
            }
        }
    }

    static bool IsEnemy(Vector2Int loc)
    {
        PlayerNode node = BlockGrid.GetTreeNode(loc);
        return node != null && current != node.Tree;
    }

    static bool IsAlly(Vector2Int loc)
    {
        PlayerNode node = BlockGrid.GetTreeNode(loc);
        return node != null && current == node.Tree;
    }

    static bool CanGrow(Vector2Int from, Vector2Int to, out (float changeScore, PlayerNode enemyNode) changes)
    {
        changes = (0, null);
        if (!BlockGrid.Contains(to) || IsAlly(to))
            return false;
        if (IsEnemy(to))
            changes.enemyNode = BlockGrid.GetTreeNode(to);
        for (Vector2Int leftOrRight = Vector2Int.left; leftOrRight.x < 2; leftOrRight.x += 2)
            if (from == to + Vector2Int.up + leftOrRight
                && (BlockGrid.GetType(to + Vector2Int.up) != BlockType.Leaf
                || BlockGrid.GetType(to + leftOrRight) != BlockType.Leaf))
            {
                if (IsAlly(to + Vector2Int.up) && IsAlly(to + leftOrRight))
                    return false;
                if (IsEnemy(to + Vector2Int.up) && IsEnemy(to + leftOrRight))
                    changes.enemyNode = BlockGrid.GetTreeNode(to + leftOrRight);
            }

        changes.changeScore = GetChangeScore(to);
        if (float.IsNaN(changes.changeScore) || current.score + changes.changeScore < 0)
            return false;
        return true;
    }

    static float GetChangeScore(Vector2Int loc)
    {
        return BlockGrid.GetType(loc) switch
        {
            BlockType.Dirt => -2,
            BlockType.Rock => -5,
            BlockType.Water => 8,
            BlockType.Twig => -4,
            BlockType.DeadTwig => -1,
            _ => float.NaN,
        } - costPenalty;
    }
}
