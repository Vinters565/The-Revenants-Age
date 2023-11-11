using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FightPath
{
    private List<Vector3> points;
    private List<FightNode> nodes;
    private List<int> stepsCost;
    private List<bool> isPassablePoints;
    public int EnergyCost { get; private set; }
    public bool IsPassable { get; private set; }

    public IReadOnlyList<Vector3> Points => points;

    public FightNode LastNode => (nodes != null && nodes.Count > 0) ? nodes[0] : null; 

    public FightPath(int energy = 0, bool isPassable = true)
    {
        stepsCost = new List<int>();
        points = new List<Vector3>();
        nodes = new List<FightNode>();
        EnergyCost = energy;
        IsPassable = isPassable;
        isPassablePoints = null;
    }

    public void AddPoint(Vector3 point, FightNode node)
    {
        points.Add(point);
        nodes.Add(node);
    }

    public void AddPointPassable(bool passable)
    {
        if (IsPassable)
            throw new InvalidOperationException("FightPath | Can't add passable point in passable path");
            //Метод должен вызываться только для непроходимых путей
        
        if (isPassablePoints == null)
            isPassablePoints = new List<bool>();
        isPassablePoints.Add(passable);
    }

    public void AddStepToEnergyCost(FightNode current, FightNode next)
    {
        if(next == null)
            return;
        if(current == null)
            throw new NullReferenceException($"Path | Node is null");
        
        int stepCost = current.StraightNeighboursContains(next) ? 1 
            : current.DiagonalNeighboursContains(next) ? 2 
            : throw new ArgumentException($"Path | trying get step cost from non neighbours");
        
        stepsCost.Add(stepCost);
        EnergyCost += stepCost;
    }

    public FightPath CutPathByEnergy(int maxEnergy)
    {
        var currentEnergyCost = 0;
        var i = 0;
        for(; i < points.Count - 1; i++)
        {
            if (currentEnergyCost > maxEnergy || (!IsPassable && !isPassablePoints[^(i + 1)]))
                break;


            currentEnergyCost += stepsCost[^(i + 1)];
        }
        
        points = points.TakeLast(i).ToList();
        nodes = nodes.TakeLast(i).ToList();
        return this;
    }
}
