using System;
using System.Collections;
using System.Collections.Generic;
using TheRevenantsAge;
using UnityEngine;

public class StateController: MonoBehaviour
{
    public static StateController Instance;

    private Dictionary<FightState, bool> availablePhase = new();
    public Dictionary<FightState, bool> AvailablePhase => availablePhase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance == this)
            Destroy(gameObject); 
    }
    
    public void MakeAvailablePhases()
    {
        availablePhase[FightState.MovePhase] = true;
        availablePhase[FightState.ShootPhase] = true;
        availablePhase[FightState.FightPhase] = true;
        availablePhase[FightState.Reload] = true;
    }

    public bool CanChangePhase()
    {
        return FightSceneController.State == FightState.Sleeping
               || FightSceneController.State == FightState.ShootPhase
               || FightSceneController.State == FightState.MovePhase
               || FightSceneController.State == FightState.FightPhase;
    }

    public void SwitchMovePhase()
    {
        if(FightSceneController.State == FightState.MovePhase)
        { 
            MovePhaseOff();
        }
        else 
        { 
            MovePhaseOn();
        }
    }

    private void MovePhaseOn()
    {
        if (CanChangePhase() && availablePhase[FightState.MovePhase])
        {
            FightSceneController.State = FightState.MovePhase;
            CameraScript.Instance.ChangeToIsometricMode(true);
            BorderController.DrawBorders();
            Debug.Log("Move Phase");
        }
    }

    private void MovePhaseOff()
    {
        FightSceneController.Instance.CurrentFightSceneCharacter.AnimationsStateController.RunAnimationOff();
        BorderController.HideBorders();
        FightSceneController.State = FightState.Sleeping;
        Debug.Log("Cancel Move Phase");
    }

    public void SwitchShootPhase()
    {
        if(FightSceneController.State == FightState.ShootPhase)
        { 
            ShootPhaseOff(); 
        }
        else
        { 
            ShootPhaseOn();
        }
        
    }

    private void ShootPhaseOn()
    {
        if (CanChangePhase() && availablePhase[FightState.ShootPhase])
        {
            FightSceneController.State = FightState.ShootPhase;
            var fightCharacter = FightSceneController.Instance.CurrentFightSceneCharacter;
            if (fightCharacter.Type == CharacterType.Ally)
            {
                fightCharacter.AnimationsStateController.AimingAnimationOn();
                CameraScript.Instance.SaveOldIsometricCameraPosition();
                var character = (IFightCharacter)fightCharacter.Entity;
                character.Aimer.ChangeCameraToAimMode();
                character.Aimer.ChangeCameraPosition(FightSceneController.Instance.CharacterObj);
            }
            Debug.Log("Shoot Phase");
        }
    }

    private void ShootPhaseOff()
    {
        FightSceneController.State = FightState.Sleeping;
        CameraScript.Instance.ChangeToIsometricMode(true);
        var fightCharacter = FightSceneController.Instance
            .CharacterObj.GetComponent<FightSceneCharacter>();
        if (fightCharacter.Type == CharacterType.Ally)
        { 
            fightCharacter.AnimationsStateController.AimingAnimationOff();
            fightCharacter.Entity.Aimer.StopDrawAiming();
        }

        Debug.Log("Cancel Shoot Phase");
    }

    public void SwitchFightPhase()
    {
        if(FightSceneController.State == FightState.FightPhase) 
        { 
            FightPhaseOff();
        }
        else 
        {
            FightPhaseOn();
        }
    }

    private void FightPhaseOn()
    {
        if (CanChangePhase() && availablePhase[FightState.FightPhase])
        {
            FightSceneController.State = FightState.FightPhase;
            CameraScript.Instance.ChangeToIsometricMode(true);
            BorderController.DrawBorders();
            Debug.Log("Fight Phase");
        }
    }

    private void FightPhaseOff()
    {
        BorderController.HideBorders();
        FightSceneController.State = FightState.Sleeping;
        Debug.Log("Cancel Fight Phase");   
    }

    public void EndPhaseOn()
    {
        if(CanChangePhase())
        {
            FightSceneController.State = FightState.EndTurnPhase;
            CameraScript.Instance.ChangeToIsometricMode(true);
            Debug.Log("End Phase");
        }
    }

    public void ReloadPhaseOn()
    {
        if (CanChangePhase())
        {
            FightSceneController.State = FightState.Reload;
            Debug.Log("Reload Phase");
        }
    }

    public void SleepingPhaseOn()
    {
        FightSceneController.State = FightState.Sleeping;
        Debug.Log("Sleeping Phase");
    }

    public void AttackCompletionOn()
    {
        FightSceneController.State = FightState.AttackCompletion;
        Debug.Log("Attack Completion");
    }

    public void AttackExecutionOn()
    {
        FightSceneController.State = FightState.AttackExecution;
        Debug.Log("Attack Execution");
    }
}
