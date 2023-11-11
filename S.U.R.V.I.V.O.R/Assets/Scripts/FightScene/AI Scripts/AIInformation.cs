using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIInformation
{
    //Служебный класс для передачи информации в BehaviourTree
    private Dictionary<FightSceneCharacter, FightPath> pathsToOpponents;
    private List<FightSceneCharacter> opponents;
    private List<FightSceneCharacter> partakers;

    public IReadOnlyDictionary<FightSceneCharacter, FightPath> PathsToOpponents => pathsToOpponents;
    public IReadOnlyList<FightSceneCharacter> Opponents => opponents;
    public IReadOnlyList<FightSceneCharacter> Partakers => partakers;

    public FightSceneCharacter CurrentCharacter { get; private set;}

    public static AIInformation GetAIInformation(List<FightSceneCharacter> opponents,
        List<FightSceneCharacter> partakers,
        FightSceneCharacter currentCharacter)
    {
        //NodesNav.FindTrackingForAllNodes(currentCharacter.GetComponent<NodesNavAgent>());
        var pathsToOpponentsDictionary = FindPathsForAllOpponents(currentCharacter, opponents);
        return new AIInformation(pathsToOpponentsDictionary, opponents, partakers, currentCharacter);
    }

    private static Dictionary<FightSceneCharacter, FightPath> 
        FindPathsForAllOpponents(FightSceneCharacter currentCharacter,
            List<FightSceneCharacter> opponents)
    {

        var pathsToOpponentsDictionary = new Dictionary<FightSceneCharacter, FightPath>();
        var currentCharacterPos = currentCharacter.transform.position;
        
        //Попытка найти путь, который возможно преодолеть за 1 ход
        foreach (var opponent in opponents)
        {
            var offset = Vector3.ClampMagnitude(currentCharacterPos - opponent.transform.position,
                0.2f);
            var targetNode = NodesNav.GetNearestNodeNearEnemy(opponent, opponent.transform.position + offset
                , false);

            NodesNav.FindPathToMove(currentCharacter, targetNode);
            var path = NodesNav.FightPath;
            if (path != null)
            {
                if((pathsToOpponentsDictionary.ContainsKey(opponent) 
                    && pathsToOpponentsDictionary[opponent].EnergyCost > path.EnergyCost)
                   || !pathsToOpponentsDictionary.ContainsKey(opponent))
                    pathsToOpponentsDictionary[opponent] = path;
            }
            
        }
        
        //Попытка найти ПРОХОДИМЫЙ путь, который нельзя пройти за 1 ход
        var passableAdditionalPaths = NodesNav.FindPathsForNodesByBFS(currentCharacter, 
            opponents.Where(opp => !pathsToOpponentsDictionary.ContainsKey(opp)).ToList(), true);
        foreach (var item in passableAdditionalPaths)
        {
            pathsToOpponentsDictionary.Add(item.Key, item.Value);
        }

        var unpassableFitConditions = new List<Func<FightNode, bool>>
        {
            (n) => n.Type == NodeType.Free,
            (n) => n.Type != NodeType.Obstacle
        };
        
        //Попытка найти НЕПРОХОДИМЫЙ путь, который нельзя пройти за 1 ход
        var unpassableAdditionalPaths = NodesNav.FindPathsForNodesByBFS(currentCharacter, 
            opponents.Where(opp => !pathsToOpponentsDictionary.ContainsKey(opp)).ToList(), false,
            unpassableFitConditions);
        foreach (var item in unpassableAdditionalPaths)
        {
            pathsToOpponentsDictionary.Add(item.Key, item.Value);
        }

        return pathsToOpponentsDictionary;
    }

    private AIInformation(Dictionary<FightSceneCharacter, FightPath> pathsToOpponents,
        List<FightSceneCharacter> opponents,
        List<FightSceneCharacter> partakers,
        FightSceneCharacter fightSceneCharacter)
    {
        this.pathsToOpponents = pathsToOpponents;
        this.opponents = opponents;
        this.partakers = partakers;
        CurrentCharacter = fightSceneCharacter;
    }
}
