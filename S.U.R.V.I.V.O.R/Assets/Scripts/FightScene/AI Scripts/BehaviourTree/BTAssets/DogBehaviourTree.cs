using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DogBehaviourTree : BehaviourTree
{
    private RootBTNode root;
    
    public override RootBTNode Root
    {
        get
        {
            if(root == null)
                InitializeRoot();
            return root;
        }
    }

    private void InitializeRoot()
    {
        root = new RootBTNode("ROOT", (decision, information) => BTNodeStatus.Succeed);
        var moveConditionNode = new ConditionBTNode("Move Condition", (decision, information) =>
        {
            if (information.CurrentCharacter.CanDoPhases(FightState.MovePhase))
            {
                decision.State = FightState.MovePhase;
                return BTNodeStatus.Succeed;
            }

            return BTNodeStatus.Failed;
        });

        var moveCheckConditionsNode = new ActionBTNode("Check Conditions to move", (decision, information) =>
        {
            var nearestOpp = information.PathsToOpponents
                .Keys.OrderBy(k => information.PathsToOpponents[k].EnergyCost)
                .FirstOrDefault();
            if (nearestOpp == null)
            {
                return BTNodeStatus.Failed;
            }

            var pathToOpp = information.PathsToOpponents[nearestOpp].CutPathByEnergy(information.CurrentCharacter.RemainingEnergy);
            
            if (pathToOpp.Points.Count <= 1)
            {
                return BTNodeStatus.Failed;
            }

            NodesNav.SetPath(pathToOpp);
            return BTNodeStatus.Succeed;
        });
        moveConditionNode.AddChild(moveCheckConditionsNode);
        
        var fightConditionNode = new ConditionBTNode("Fight Condition", (decision, information) =>
        {
            if (information.CurrentCharacter.CanDoPhases(FightState.FightPhase))
            {
                decision.State = FightState.FightPhase;
                return BTNodeStatus.Succeed;
            }
            
            return BTNodeStatus.Failed;
        });

        var findFightTargetNode = new ActionBTNode("Find target to fight", (decision, information) =>
        {
            var sortedOpponents = information.PathsToOpponents
                .Keys.OrderBy(k => information.PathsToOpponents[k].EnergyCost)
                .ToList();
            var currentCharacterPos = information.CurrentCharacter.transform.position;
            foreach (var opp in sortedOpponents)
            {
                var path = information.PathsToOpponents[opp];
                if (path != null && path.EnergyCost <= information.CurrentCharacter.RemainingEnergy && path.IsPassable)
                {
                    if (path.Points.Count == 0)
                    {
                        path = new FightPath(path.EnergyCost,
                            isPassable: path.IsPassable);
                        path.AddPoint(currentCharacterPos, information.CurrentCharacter.GetComponent<NodesNavAgent>().RootNode);
                    }

                    decision.Target = opp.transform;
                    NodesNav.SetPath(path);
                    return BTNodeStatus.Succeed;
                }
            }
            
            return BTNodeStatus.Failed;
        });
        fightConditionNode.AddChild(findFightTargetNode);

        var endPhaseNode = new ActionBTNode("End Turn", (decision, information) =>
        {
            decision.State = FightState.EndTurnPhase;
            return BTNodeStatus.Succeed;
        });
        
        root.AddChild(fightConditionNode);
        root.AddChild(moveConditionNode);
        root.AddChild(endPhaseNode);
    }
}
