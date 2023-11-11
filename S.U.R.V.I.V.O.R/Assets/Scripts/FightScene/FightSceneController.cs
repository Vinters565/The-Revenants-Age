using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using Extension;
using TheRevenantsAge;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class FightSceneController : MonoBehaviour
{
    public static FightSceneController Instance { get; private set; }
    public static Fight Current { get; set; }
    public static FightState State { get; set; }
    public static Queue<Transform> CharactersQueue { get; private set; }

    public Transform CharacterObj { get; private set; }
    
    [SerializeField] private EventSystem eventSystem;

    private Camera mainCamera;
    
    private List<Transform> fightEntityObjs;
    
    private FightSceneCharacter fightSceneCharacter;
    private GameObject currentCharacterNodeObj;
    
    private List<Vector3> allySpawnPoints;
    private List<Vector3> enemySpawnPoints;
    
    private Transform targetObjToAction;

    public UnityEvent<IFightCharacter> chosenPlayerCharacterChanged;

    public IReadOnlyList<Transform> FightEntityObjs => fightEntityObjs;

    public FightSceneCharacter CurrentFightSceneCharacter
    {
        get => fightSceneCharacter;
        private set
        {
            fightSceneCharacter = value;
            if (fightSceneCharacter.Type == CharacterType.Ally)
            {
                chosenPlayerCharacterChanged.Invoke(fightSceneCharacter.Entity as IFightCharacter);
            }
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        mainCamera = Camera.main;
        CreateSpawnPointsLists();
        CreateCharactersList();
        InitializeCharactersQueue();
        AI.MakeFightSceneCharactersLists(CharactersQueue.Select(characterObj =>
            characterObj.GetComponent<FightSceneCharacter>()));
        NodesNav.InitializeNodesLists();
        UIController.Instance.CreateUI();

        StateController.Instance.MakeAvailablePhases();
        CharacterObj = CharactersQueue.Dequeue();
        CurrentFightSceneCharacter = CharacterObj.GetComponent<FightSceneCharacter>();
        AI.CurrentCharacterObj = CharacterObj;

        RedrawAreas();
        State = FightState.Sleeping;
        UpdateNearestNodeToCurrentCharacter();
        if (CurrentFightSceneCharacter.Type == CharacterType.Ally)
            chosenPlayerCharacterChanged.Invoke(CurrentFightSceneCharacter.Entity as IFightCharacter);
    }

    private void Update()
    {
        NodesNav.ErasePath();

        if (CurrentFightSceneCharacter.Type == CharacterType.Ally)
        {
            var point = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            var cameraRay = mainCamera.ScreenPointToRay(point);
            var hitted = Physics.Raycast(cameraRay, out var hit);

            switch (State)
            {
                case FightState.MovePhase:
                    CalculateAvailablePathToPoint(hit, false);
                    break;
                case FightState.FightPhase:
                    CalculateAvailablePathToPoint(hit, true);
                    break;
                case FightState.ShootPhase:
                    TakeAim();
                    break;
                case FightState.Reload:
                    Reload();
                    break;
                case FightState.EndTurnPhase:
                    EndTurn();
                    break;
            }

            if (Input.GetMouseButtonDown(0) && !eventSystem.IsPointerOverGameObject() && hitted)
                MakeAction(State, hit.transform);
        }
        else
        {
            if (!AI.AIMadeDecision)
            {
                var decision = AI.MakeDecision();
                MakeAction(decision.State, decision.Target);
            }
        }

        if (State == FightState.Moving || State == FightState.Fighting)
        {
            var isEndMove = CurrentFightSceneCharacter.ContinueMove();
            if (isEndMove)
                CharacterReachTarget();
        }

        if (State == FightState.AttackCompletion)
        {
            AttackCompletion();
        }
        
        if(State == FightState.Completion)
            TurnCompletion();
    }

    private void MakeAction(FightState state, Transform targetObj)
    {
        switch (state)
        {
            case FightState.MovePhase:
                MoveCharacter();
                break;
            case FightState.FightPhase:
                Fight(targetObj);
                break;
            case FightState.ShootPhase:
                Shoot();
                break;
            case FightState.Reload:
                Reload();
                break;
            case FightState.EndTurnPhase:
                EndTurn();
                break;
        }
    }

    private void CreateSpawnPointsLists()
    {
        var spawnPointsParent = GameObject.Find("SpawnPoints");
        if (spawnPointsParent == null)
            throw new NullReferenceException("No \"SpawnPoints\" object on scene");

        var spawnPoints = spawnPointsParent.transform
            .GetChildes()
            .Select(c => c.gameObject.GetComponent<FightSpawnPoint>())
            .ToList();
        
        allySpawnPoints = spawnPoints
            .Where(p => p.Type == CharacterType.Ally)
            .Select(p => p.transform.position)
            .ToList();
        enemySpawnPoints = spawnPoints
            .Where(p => p.Type == CharacterType.Enemy)
            .Select(p => p.transform.position)
            .ToList();
    }

    private void CreateCharactersList()
    {
        fightEntityObjs = new();
        foreach (var characterData in Current.Ally)
        {
            var character = characterData.InstantiateGameObj3D();
            var objTransform = character.gameObject.transform;
            NodesNav.MoveFightSceneCharacterToSpawnPoint(objTransform, allySpawnPoints[^1]);
            allySpawnPoints.RemoveAt(allySpawnPoints.Count - 1);
            fightEntityObjs.Add(objTransform);
        }

        foreach (var entity in Current.Enemies)
        {
            var entityObj = Instantiate(entity, Vector3.zero, Quaternion.identity);
            var objTransform = entityObj.gameObject.transform;
            objTransform.localPosition = Vector3.zero;
            NodesNav.MoveFightSceneCharacterToSpawnPoint(objTransform, enemySpawnPoints[^1]);
            enemySpawnPoints.RemoveAt(enemySpawnPoints.Count - 1);
            fightEntityObjs.Add(objTransform);
        }

        fightEntityObjs = fightEntityObjs
            .OrderByDescending(c => c.GetComponent<FightSceneCharacter>().Initiative)
            .ToList();
    }


    private void InitializeCharactersQueue()
    {
        CharactersQueue = new Queue<Transform>(fightEntityObjs);
        Debug.Log("CharacterQueue Count: " + CharactersQueue.Count);
    }

    private void CharacterReachTarget()
    {
        NodesNav.ClearPath();
        currentCharacterNodeObj = NodesNav.GetNearestNode(CharacterObj.position).gameObject;
        CameraScript.Instance.CameraObserveTo(null);
        CurrentFightSceneCharacter.ReachTarget();
        if (State == FightState.Fighting && CurrentFightSceneCharacter.CanDoPhases(FightState.FightPhase))
        {
            CurrentFightSceneCharacter.MeleeAttack();
            StateController.Instance.AttackExecutionOn();
        }
        else
        {
            AI.AIMadeDecision = false;
            RedrawAreas();
            StateController.Instance.SleepingPhaseOn();
        }
    }

    private void TurnCompletion()
    {
        if(CameraScript.Instance.CameraReachedGoal())
            StateController.Instance.SleepingPhaseOn();
    }

    private void AttackCompletion()
    {
        AI.AIMadeDecision = false;
        RedrawAreas();
        StateController.Instance.SleepingPhaseOn();
    }

    private void EndTurn()
    {
        State = FightState.Completion;
        CharactersQueue.Enqueue(CharacterObj);
        CharacterObj = GetNextCharacter();
        AI.CurrentCharacterObj = CharacterObj;


        StateController.Instance.MakeAvailablePhases();
        currentCharacterNodeObj.GetComponent<FightNode>().SetNodeDefault();
        currentCharacterNodeObj = NodesNav.GetNearestNode(CharacterObj.transform.position).gameObject;
        RedrawAreas();
        
        UIController.Instance.ChangeActiveCard();
        AI.AIMadeDecision = false;

        CameraScript.Instance.MoveCameraToTarget(CharacterObj);
        // Идет фаза Completion
    }

    private Transform GetNextCharacter()
    {
        CurrentFightSceneCharacter.ResetEnergy();
        CurrentFightSceneCharacter.ResetActionPoints();

        var nextCharacter = CharactersQueue.Dequeue();
        while (nextCharacter == null || !nextCharacter.GetComponent<FightSceneCharacter>().Alive)
        {
            nextCharacter = CharactersQueue.Dequeue();
        }

        CurrentFightSceneCharacter = nextCharacter.GetComponent<FightSceneCharacter>();

        return nextCharacter;
    }

    private void MoveCharacter()
    {
        if (CurrentFightSceneCharacter.CanDoPhases(FightState.MovePhase))
        {
            var successStart = CurrentFightSceneCharacter.StartMove();
            if (successStart)
            {
                CameraScript.Instance.CameraObserveTo(CharacterObj);
                State = FightState.Moving;
                //StateController.Instance.AvailablePhase[FightState.MovePhase] = false;
            }
        }
        else
            Debug.Log("Нельзя больше сходить в этом ходу");
    }

    private void Fight(Transform targetObj)
    {
        var enemyObj = GetFightCharacterFromBodyPart(targetObj);
        if (enemyObj
            && enemyObj.transform != CharacterObj
            && enemyObj.Type != CurrentFightSceneCharacter.Type
            && ((CurrentFightSceneCharacter.CanDoPhases(FightState.MovePhase) && CurrentFightSceneCharacter.Type != CharacterType.Ally)
                || CurrentFightSceneCharacter.CanDoPhases(FightState.MovePhase, FightState.FightPhase)))
        {
            CurrentFightSceneCharacter.TargetToHit = targetObj.position;
            var successStart = CurrentFightSceneCharacter.StartMove();
            if (successStart)
            {
                CameraScript.Instance.CameraObserveTo(CharacterObj);
                State = FightState.Fighting;
                //StateController.Instance.AvailablePhase[FightState.FightPhase] = false;
                //StateController.Instance.AvailablePhase[FightState.ShootPhase] = false;
                //StateController.Instance.AvailablePhase[FightState.MovePhase] = false;
            }
            else if(NodesNav.IsFightPathDegenerate())
            {
                State = FightState.Fighting;
                CharacterReachTarget();
            }
        }
        else
            Debug.Log("Недоступная зона или Необходимо указать на врага");
    }

    private void Shoot()
    {
        if (CurrentFightSceneCharacter.CanDoPhases(FightState.ShootPhase))
        {
            State = FightState.Shooting;
            StateController.Instance.AttackExecutionOn();
            CurrentFightSceneCharacter.MakeShoot();
            if (CurrentFightSceneCharacter.Type == CharacterType.Ally)
            {
                CameraScript.Instance.ChangeToIsometricMode(false);
                StaticLineDrawer.Instance.DeleteLine();
            }

            //StateController.Instance.AvailablePhase[FightState.FightPhase] = false;
            //StateController.Instance.AvailablePhase[FightState.ShootPhase] = true;
        }
    }

    private void Reload()
    {
        if (CurrentFightSceneCharacter.CanDoPhases(FightState.Reload))
        {
            State = FightState.Reloading;

            CurrentFightSceneCharacter.Reload();

            State = FightState.Sleeping; //Перенести в анимации
            AI.AIMadeDecision = false;
            //StateController.Instance.AvailablePhase[FightState.Reload] = false;
        }
    }


    public void DeleteDeathCharacterFromQueue(FightSceneCharacter sceneCharacter)
    {
        Debug.Log("Delete");
        if (sceneCharacter == CurrentFightSceneCharacter)
        {
            Debug.Log("Character Already Died | Need fix it");
            CharacterObj = GetNextCharacter();
            AI.CurrentCharacterObj = CharacterObj;


            StateController.Instance.MakeAvailablePhases();
            currentCharacterNodeObj.GetComponent<FightNode>().SetNodeDefault();
            currentCharacterNodeObj = NodesNav.GetNearestNode(CharacterObj.transform.position).gameObject;
            RedrawAreas();
        }

        var newQueue = new Queue<Transform>();
        var hasEnemies = CurrentFightSceneCharacter.Type == CharacterType.Enemy && CurrentFightSceneCharacter.Alive;
        var hasAllies = CurrentFightSceneCharacter.Type == CharacterType.Ally && CurrentFightSceneCharacter.Alive;
        while (CharactersQueue.Count > 0)
        {
            var charObj = CharactersQueue.Dequeue();
            var fightCharacter = charObj.GetComponent<FightSceneCharacter>();
            if (charObj != null && fightCharacter != sceneCharacter)
            {
                newQueue.Enqueue(charObj);
                if (fightCharacter.Type == CharacterType.Ally) hasAllies = true;
                if (fightCharacter.Type == CharacterType.Enemy) hasEnemies = true;
            }
        }

        CheckFightEnd(hasAllies, hasEnemies);
        CharactersQueue = newQueue;
    }

    private void CheckFightEnd(bool hasAllies, bool hasEnemies)
    {
        if (!hasAllies)
            SceneManager.LoadScene((int)SceneName.DeadScene);
        if (!hasEnemies && hasAllies)
            UIController.Instance.ShowWinPanel();
        
    }

    public void UpdateNearestNodeToCurrentCharacter()
    {
        currentCharacterNodeObj = NodesNav.GetNearestNode(CharacterObj.position).gameObject;
    }

    public void RedrawAreas()
    {
        NodesNav.CleanAreasLists();
        if (StateController.Instance.AvailablePhase[FightState.MovePhase]
            || StateController.Instance.AvailablePhase[FightState.FightPhase])
        {
            NodesNav.RecalculateAreas(CharactersQueue, CharacterObj);
        }
    }

    private void CalculateAvailablePathToPoint(RaycastHit hit, bool isForFighting)
    {
        var targetObj = GetFightCharacterFromBodyPart(hit.transform);
        if (hit.transform != null &&
            ((targetObj && isForFighting) || !isForFighting))
        {
            if (isForFighting)
                NodesNav.FindPathToMove(CurrentFightSceneCharacter, NodesNav.GetNearestNodeNearEnemy(targetObj, hit.point));
            else
                NodesNav.FindPathToMove(CurrentFightSceneCharacter, NodesNav.GetNearestNode(hit.point));

            if (NodesNav.FightPath != null && NodesNav.FightPath.Points.Count != 0)
                NodesNav.DrawPath(CurrentFightSceneCharacter);
        }
    }

    private void TakeAim()
    {
        CurrentFightSceneCharacter.TakeAim();
    }

    private FightSceneCharacter GetFightCharacterFromBodyPart(Transform bodyPartObj)
    {
        var currentObj = bodyPartObj;
        while (currentObj)
        {
            var result = currentObj.GetComponent<FightSceneCharacter>();
            if (result)
                return result;
            currentObj = currentObj.parent;
        }

        return null;
    }
}