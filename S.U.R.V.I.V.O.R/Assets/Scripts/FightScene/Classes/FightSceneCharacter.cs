using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TheRevenantsAge;
using UnityEngine.Serialization;

public class FightSceneCharacter : MonoBehaviour
{
    private static readonly int MOVE_ACTION_POINTS_COST = 1;
    private static readonly int SHOOT_ACTION_POINTS_COST = 2;
    private static readonly int FIGHT_ACTION_POINTS_COST = 2;
    private static readonly int RELOAD_ACTION_POINTS_COST = 1;
    
    public IFightEntity Entity { get; private set; }
    [field:SerializeField] public CharacterType Type { get; private set; }
    [SerializeField] private int maxActionPoints;

    public float Initiative => Entity.Initiative;
    public int Energy => Entity.SpeedInFightScene;
    public int RemainingEnergy { get; set; }

    private int actionPoints;
    public int ActionPoints
    {
        get => actionPoints;
        set
        {
            //UIController.Instance.RedrawActionsPoints(value, maxActionPoints);
            actionPoints = value;
        }
    }
    public bool Alive { get; private set; } = true;
    public Vector3? TargetToHit { get; set; }
    public float radius => 10f;
    public AnimationsStateController AnimationsStateController { get; private set; }
    public ArmatureController ArmatureController { get; private set; }

    private AttackType attackType = AttackType.DefaultAttack;

    private void Awake()
    {
        AnimationsStateController = GetComponent<AnimationsStateController>();
        ArmatureController = GetComponent<ArmatureController>();
        Entity = GetComponent<IFightEntity>();
        RemainingEnergy = Energy;
        ActionPoints = maxActionPoints;
    }
    
    private void OnEnable()
    {
        ArmatureController.SetActiveAllRigidBodies(false);
        Entity.Body.Died += OnDied;
        Entity.Body.Died += FightSceneController.Instance.RedrawAreas;
        Entity.Body.Died += FightSceneController.Instance.UpdateNearestNodeToCurrentCharacter;
        Entity.Body.Died += AI.RefreshFightCharactersLists;
        Entity.WhenDamagedOrHealed += UIController.Instance.DrawDamage;
    }
    
    private void OnDisable()
    {
        Entity.Body.Died -= OnDied;
        Entity.Body.Died -= FightSceneController.Instance.RedrawAreas;
        Entity.Body.Died -= FightSceneController.Instance.UpdateNearestNodeToCurrentCharacter;
        Entity.Body.Died -= AI.RefreshFightCharactersLists;
        Entity.WhenDamagedOrHealed -= UIController.Instance.DrawDamage;
    }

    public bool CanDoPhases(params FightState[] states)
    {
        var canDo = true;
        var currentCost = 0;
        foreach (var state in states)
        {   
            switch (state)
            {
                case FightState.MovePhase:
                    canDo &= StateController.Instance.AvailablePhase[state] && actionPoints >= currentCost + MOVE_ACTION_POINTS_COST;
                    currentCost += MOVE_ACTION_POINTS_COST;
                    break;
                case FightState.FightPhase:
                    canDo &= StateController.Instance.AvailablePhase[state] && actionPoints >= currentCost + FIGHT_ACTION_POINTS_COST;
                    currentCost += FIGHT_ACTION_POINTS_COST;
                    break;
                case FightState.ShootPhase:
                    canDo &= StateController.Instance.AvailablePhase[state] && actionPoints >= currentCost + SHOOT_ACTION_POINTS_COST;
                    currentCost += SHOOT_ACTION_POINTS_COST;
                    break;
                case FightState.Reload:
                    canDo &= actionPoints >= currentCost + RELOAD_ACTION_POINTS_COST;
                    currentCost += RELOAD_ACTION_POINTS_COST;
                    break;
                default:
                    return false;
            }
        }
        return canDo;
    }

    public void MakeShoot()
    {
        Attack();
        ActionPoints -= SHOOT_ACTION_POINTS_COST;
        Entity.Aimer?.StopDrawAiming();
    }

    public void MeleeAttack()
    {
        Debug.Log("Hit");
        transform.LookAt(new Vector3(TargetToHit.Value.x,
            transform.position.y,
            TargetToHit.Value.z));
        Attack(attackType);

        ActionPoints -= FIGHT_ACTION_POINTS_COST;
    }

    private void Attack(AttackType type = AttackType.DefaultAttack)
    {
        //AnimationsStateController.AimingAnimationOn();
        Entity.Attack(TargetToHit.Value, type);
        TargetToHit = null;
        StartCoroutine(AimingAnimationOff());
        
        //Дальше идет фаза AttackExecution, и в аниматоре вызывается фаза AttackCompletion
    }

    private IEnumerator AimingAnimationOff()
    {
        yield return new WaitForSeconds(1f);
        AnimationsStateController.AimingAnimationOff();
    }

    public void AttackCompletion()
    {
        StateController.Instance.AttackCompletionOn();
    }

    public void Reload()
    {
        if (Type == CharacterType.Ally)
        {   
            (AnimationsStateController as CharacterAnimationController).Reload();
            var fightCharacter = Entity as BaseFightCharacter;
            fightCharacter.ReloadChosenWeapon();

            ActionPoints -= RELOAD_ACTION_POINTS_COST;
        }
    }

    public bool StartMove()
    {
        if (NodesNav.FightPath == null)
        {
            Debug.Log("Не найден путь");
            return false;
        }

        if (NodesNav.FightPath != null && NodesNav.FightPath.Points.Count != 1) 
                                       //&& NodesNav.FightPath.EnergyCost <= RemainingEnergy + 1)
        {
            AnimationsStateController.RunAnimationOn();
            NodesNav.StartMoveCharacter(transform);
            
            ActionPoints -= MOVE_ACTION_POINTS_COST;
            return true;
        }
        else
        {
            Debug.Log($"Невозможно сходить на такое расстояние | Points Count {NodesNav.FightPath.Points.Count}");
            return false;
        }
    }

    public bool ContinueMove()
    {
        if (!NodesNav.IsEndMoveCharacter())
            NodesNav.MoveCharacterByPath(transform);
        return NodesNav.IsEndMoveCharacter();
    }

    public void ReachTarget()
    {
        StopAim();
        AnimationsStateController.RunAnimationOff();
    }

    public void ResetEnergy()
    {
        RemainingEnergy = Energy;
    }

    public void ResetActionPoints()
    {
        ActionPoints = maxActionPoints;
    }

    public void TakeAim()
    {
        if (Type == CharacterType.Ally)
        {
            var character = (IFightCharacter)Entity;
            TargetToHit = character.Aimer.Aim();
            character.Aimer.DrawAimLine(TargetToHit.Value);
        }
        else
        {
            //TODO Реализовать прицеливание для ИИ;
        }
    }

    public void StopAim()
    {
        if (Type == CharacterType.Ally)
            Entity.Aimer?.StopDrawAiming();
    }

    public bool SetChosenWeapon(ChosenWeaponTypes weaponType)
    {
        if (Type == CharacterType.Ally)
        {
            var character = Entity as ICharacter;
            Weapon weapon;
            switch (weaponType)
            {
                case ChosenWeaponTypes.Primary:
                    weapon = character.PrimaryGun;
                    break;
                case ChosenWeaponTypes.Secondary:
                    weapon = character.SecondaryGun;
                    break;
                case ChosenWeaponTypes.Melee:
                    weapon = character.MeleeWeapon;
                    break;
                default:
                    weapon = null;
                    break;
            }

            if (weapon != null && character.ChosenWeaponType != weaponType)
            {
                character.SetChosenWeapon(weaponType);
                return true;
            }
            character.SetChosenWeapon(ChosenWeaponTypes.None);
            
        }

        return false;
    }

    public void SetAttackType(AttackType type)
    {
        attackType = type;
    }

    private void OnDied()
    {
        Debug.Log("--DIED--");
        Alive = false;
        FightSceneController.Instance.DeleteDeathCharacterFromQueue(this);
        UIController.Instance.DeleteDeathCharacterCard(this);
        AnimationsStateController.DisableAnimator();
        StartCoroutine(StartDisappearCharacter(gameObject));
    }

    private IEnumerator StartDisappearCharacter(GameObject character)
    {
        ArmatureController.SetActiveAllRigidBodies(true);
        yield return new WaitForSeconds(2f);
        ArmatureController.SetActiveAllColliders(false);
        ArmatureController.SetActiveAllRigidBodies(false);
        var offset = 0;
        while (offset < 400)
        {
            yield return null;
            character.transform.Translate(0, -0.005f, 0);
            offset++;
        }
        Destroy(gameObject);
    }
}