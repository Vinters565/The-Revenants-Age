using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class AI 
{
    public static Transform CurrentCharacterObj { get; set; }
    public static bool AIMadeDecision { get; set; }

    private static List<FightSceneCharacter> opponents;
    private static List<FightSceneCharacter> partakers;
    private static Dictionary<FightSceneCharacter, List<Vector3>> pathsToOpponents;

    public static void MakeFightSceneCharactersLists(IEnumerable<FightSceneCharacter> characters)
    {
        opponents = new List<FightSceneCharacter>();
        foreach (var character in characters
                     .Where(opp => opp.Type == CharacterType.Ally))
        {
            opponents.Add(character);
        }
        
        partakers = new List<FightSceneCharacter>();
        foreach (var character in characters
                     .Where(opp => opp.Type == CharacterType.Enemy))
        {
            partakers.Add(character);
        }
    }
    
    public static void RefreshFightCharactersLists() //Подписан на Body.Died;
    {
        opponents = opponents.Where(oppT =>
        {
            var fightCharacter = oppT.GetComponent<FightSceneCharacter>();
            return fightCharacter && fightCharacter.Type == CharacterType.Ally && fightCharacter.Alive;
        })
            .ToList();
        
        partakers = partakers.Where(oppT =>
            {
                var fightCharacter = oppT.GetComponent<FightSceneCharacter>();
                return fightCharacter && fightCharacter.Type == CharacterType.Enemy && fightCharacter.Alive;
            })
            .ToList();
    }

    public static Decision MakeDecision()
    {
        if (!StateController.Instance.CanChangePhase())
            return new Decision(FightSceneController.State, null);
        
        AIMadeDecision = true;
        var aiInformation = AIInformation.GetAIInformation(opponents, partakers,
            CurrentCharacterObj.GetComponent<FightSceneCharacter>());

        return CurrentCharacterObj.GetComponent<Behaviour>().MakeDecision(aiInformation);
    }
}
