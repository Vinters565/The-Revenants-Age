using System.Collections.Generic;
using System.Linq;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    public class Fight
    {
        public readonly List<GameObject> Enemies;
        public readonly List<CharacterState> Ally;
        public readonly SceneName SceneName;

        public Fight(FightInfo fightInfo)
        {
            GlobalMapController.UpdateState();
            Enemies = fightInfo.Enemies
                .Select(e => e.gameObject)
                .ToList();
            Ally = GlobalMapController.State.ChosenGroup.currentGroupMembers.ToList();
            SceneName = fightInfo.SceneName;
        }


        public void Initialization()
        {
            FightSceneController.Current = this;
            SceneTransition.LoadScene(SceneName);
        }

        public void End(List<ICharacter> characters)
        {
            if (characters == null || characters.Count == 0)
                GlobalMapController.State.groups.Remove(GlobalMapController.State.ChosenGroup);
            else
            {
                var characterStates = characters
                    .Select(x => x.CreateState())
                    .ToArray();

                GlobalMapController.State.ChosenGroup.currentGroupMembers = characterStates;
                GlobalMapController.State.ChosenGroup.isLooting = true;
            }

            SceneTransition.LoadScene(SceneName.GlobalMapScene);
        }
    }
}