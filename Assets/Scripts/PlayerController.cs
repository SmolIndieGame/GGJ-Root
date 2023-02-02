using System.Collections.Generic;
using UnityEngine;

public static class PlayerController
{
    const float InitialPenalty = 4;
    const float PenaltyMultiplier = 2;
    const float InitialScore = 20;
    const int PlayerStartX = 5;

    static PlayerTree p1;
    static PlayerTree p2;

    public static readonly List<Vector2Int> deepWaterLocations = new();
    public static readonly List<Vector2Int> victoryLocations = new();
    
    #region Fields For Handling Turns
    static PlayerTree current;
    static PlayerNode touchingLeaf;
    static readonly Dictionary<Vector2Int, (float changeScore, PlayerNode enemyNode)> growableLocs = new();
    static bool branching;
    static float costPenalty;
    #endregion

    public static VisualGuider visualGuider { private get; set; }

    public static event System.Action<float> P1ScoreChange;
    public static event System.Action<float> P2ScoreChange;
    public static event System.Action<bool> CanEndTurnChange;
    public static event System.Action<int> OnEndTurn;
    public static event System.Action<int> OnGameOver;

    public static void Init(PlayerTextureSet p1Color, PlayerTextureSet p2Color)
    {
        p1 = new PlayerTree(new Vector2Int(PlayerStartX, 0), InitialScore, p1Color);
        p2 = new PlayerTree(new Vector2Int(BlockGrid.MapWidth - PlayerStartX - 1, 0), InitialScore, p2Color);

        var tmpLoc = new Vector2Int(PlayerStartX, 0);
        BlockGrid.ExtendLeaf(tmpLoc, tmpLoc, p1.Root);
        tmpLoc = new Vector2Int(BlockGrid.MapWidth - PlayerStartX - 1, 0);
        BlockGrid.ExtendLeaf(tmpLoc, tmpLoc, p2.Root);

        current = p1;
        touchingLeaf = null;
        growableLocs.Clear();
        branching = false;
        costPenalty = 0;

        P1ScoreChange?.Invoke(p1.score);
        P2ScoreChange?.Invoke(p2.score);
    }

    public static void EndCurrentPlayerTurn()
    {
        if (costPenalty == 0)
            return;
        if (touchingLeaf != null)
            Cancel();

        foreach (var loc in deepWaterLocations)
            DominationReward(loc);

        if (p1 == current)
            P1ScoreChange?.Invoke(current.score);
        else
            P2ScoreChange?.Invoke(current.score);

        current = current == p1 ? p2 : p1;
        costPenalty = 0;
        CanEndTurnChange?.Invoke(false);
        OnEndTurn?.Invoke(current == p1 ? 1 : 2);
    }

    static void DominationReward(Vector2Int loc)
    {
        if (!BlockGrid.Contains(loc) || !IsAlly(loc))
            return;

        current.score++;
    }

    static void VictoryDetection(Vector2Int loc)
    {
        if (!BlockGrid.Contains(loc) || !IsAlly(loc))
            return;

        OnGameOver?.Invoke(current == p1 ? 1 : 2);
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
            P1ScoreChange?.Invoke(current.score);
        else
            P2ScoreChange?.Invoke(current.score);
        CanEndTurnChange?.Invoke(true);

        touchingLeaf = null;
        branching = false;
        growableLocs.Clear();
        visualGuider.RemoveAllGuide();

        if (current.score < 0)
            OnGameOver?.Invoke(current == p1 ? 2 : 1);

        foreach (var item in victoryLocations)
            VictoryDetection(item);

        if (changes.enemyNode == p1.Root)
            OnGameOver?.Invoke(2);
        if (changes.enemyNode == p2.Root)
            OnGameOver?.Invoke(1);
    }

    static void DestroySubTree(PlayerNode node)
    {
        if (node == null)
            return;
        if (node.Parent != null)
        {
            node.Parent.RemoveChild(node);
            BlockGrid.SetBlockType(node.Parent.Location, BlockGrid.GetType(node.Parent.Location));
            if (node.Parent.ChildrenCount == 0)
            {
                node.Tree.NewLeaf(node.Parent);
                BlockGrid.SetTwigToLeaf(node.Parent.Location);
            }
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
        {
            if (from != to + Vector2Int.up + leftOrRight)
                continue;
            PlayerNode upNode = BlockGrid.GetTreeNode(to + Vector2Int.up);
            PlayerNode horizontalNode = BlockGrid.GetTreeNode(to + leftOrRight);
            if (upNode == null || upNode != horizontalNode?.Parent)
                continue;

            if (IsAlly(to + Vector2Int.up))
                return false;
            if (IsEnemy(to + Vector2Int.up))
                changes.enemyNode = horizontalNode;
        }

        changes.changeScore = GetChangeScore(to);
        if (float.IsNaN(changes.changeScore))
            return false;
        return true;
    }

    public static float GetCurrentScore() => current.score;

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
