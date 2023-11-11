// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Data;
// using _3DCharacterExtensions;
// using _3DCharacterExtensions.ForAnimations;
// using Model.Items.Weapons.MeleeWeapons;
// using Model.ServiceClasses;
// using Model.Weapons;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.Serialization;
//
// namespace Model.GameEntity
// {
//     public class Entity : MonoBehaviour
//     {
//         [field: SerializeField] public Body Body { get; private set; }
//         [SerializeField] [Min(1)] protected float initiative = 10;
//         [SerializeField] [Min(1)] private int speedInFightScene = 1;
//         [SerializeField] private List<MeleeWeaponTrigger> meleeAttackTriggers;
//
//         private AnimationsStateController animationsStateController;
//         public virtual Aimer Aimer => null;
//         public event Action<float, Vector3> WhenDamagedOrHealed;
//         public event Action<float, Vector3> WhenDamagedOrRecoveryArmor;
//
//         public UnityEvent WhenDefaultAttacked;
//
//         protected virtual void Awake()
//         {
//             if (Body == null)
//                 throw new ConstraintException("Invalid!");
//             animationsStateController = GetComponent<AnimationsStateController>();
//             if (animationsStateController == null)
//                 throw new ConstraintException("Invalid!");
//             Body.WhenDamagedOrHealed += BodyOnWhenDamagedOrHealed;
//             if (Body is IWearClothes wearClothesBody)
//             {
//                 wearClothesBody.WhenDamagedOrRecoveryArmor += WearClothesBodyOnWhenDamagedOrRecoveryArmor;
//             }
//         }
//
//         protected virtual void OnEnable()
//         {
//             foreach (var trigger in meleeAttackTriggers)
//             {
//                 trigger.WhenAttacked.AddListener(InvokeWhenDefaultAttacked);
//             }
//         }
//
//         protected virtual void OnDisable()
//         {
//             foreach (var trigger in meleeAttackTriggers)
//             {
//                 trigger.WhenAttacked.RemoveListener(InvokeWhenDefaultAttacked);
//             }
//         }
//
//         public virtual float Initiative
//         {
//             get => initiative;
//             set
//             {
//                 if (value < 0)
//                     throw new ConstraintException($"{nameof(initiative)} < 0");
//                 initiative = (float)Math.Round(value,1);
//             }
//         }
//         
//         public void UpDamageToMeleeAttacks(float added)
//         {
//             foreach (var meleeAttackTrigger in meleeAttackTriggers)
//                 meleeAttackTrigger.Damage += added;
//             
//         }
//         public int SpeedInFightScene
//         {
//             get => speedInFightScene;
//             set
//             {
//                 if (value < 0)
//                     throw new ConstraintException($"{nameof(speedInFightScene)} < 0");
//                 speedInFightScene = value;
//             }
//         }
//
//         public virtual void Attack(Vector3 targetPoint)
//         {
//             animationsStateController.TriggerAttack();
//         }
//         
//         private void WearClothesBodyOnWhenDamagedOrRecoveryArmor(float diff, Vector3 pos)
//         {
//             WhenDamagedOrRecoveryArmor?.Invoke(diff, pos);
//         }
//
//         private void BodyOnWhenDamagedOrHealed(float diff, Vector3 pos)
//         {
//             WhenDamagedOrHealed?.Invoke(diff, pos);
//         }
//
//         private void InvokeWhenDefaultAttacked()
//         {
//             WhenDefaultAttacked.Invoke();
//         }
//     }
// }