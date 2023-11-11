using System;
using UnityEngine;

namespace TheRevenantsAge
{
    public class AttackEnd: StateMachineBehaviour
    {
        private FightSceneCharacter character;
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            character ??= animator.GetComponent<FightSceneCharacter>();
            character.AttackCompletion();
        }
    }
}