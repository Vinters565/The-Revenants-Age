using System;
using System.Collections.Generic;
using System.Linq;
using Interface.CraftLayerLogic;
using Interface.GroupLayerLogic;
using Interface.InterfaceStates;
using Interface.Menu.ForConsole;
using Interface.PlayerInfoLayer;
using TheRevenantsAge;
using UnityEngine;

public sealed class InterfaceController : MonoBehaviour
{
    public static InterfaceController Instance { get; private set; }

    private readonly StateMachine interfaceStateMachine = new();
    private NothingLayerState NothingState { get; set; }

    private GameConsoleLayerState GameConsoleState { get; set; }
    private CraftLayerState CraftState { get; set; }
    private CharactersLayerState CharactersState { get; set; }
    private GroupLayerState GroupState { get; set; }
    private CharacterLayerState FirstCharacterState { get; set; }
    private CharacterLayerState SecondCharacterState { get; set; }
    private CharacterLayerState ThirdCharacterState { get; set; }
    private CharacterLayerState FourthCharacterState { get; set; }

    [SerializeField] private GameObject mainInfoPanelLayer;
    [SerializeField] private GameObject groupButtonsLayer;
    [SerializeField] private GameObject groupInfoLayer;
    [SerializeField] private GameObject charactersButtonsLayer;
    [SerializeField] private PlayerLayerLogic firstPlayerLayer;
    [SerializeField] private PlayerLayerLogic secondPlayerLayer;
    [SerializeField] private PlayerLayerLogic thirdPlayerLayer;
    [SerializeField] private PlayerLayerLogic fourthPlayerLayer;
    [SerializeField] private CraftLayerLogic craftLayerLogic;

    [SerializeField] private CharactersPlateLayerLogic charactersPlateLayerLogic;

    public GameObject MainInfoPanelLayer => mainInfoPanelLayer;
    public GameObject GroupButtonsLayer => groupButtonsLayer;
    public GameObject GroupInfoLayer => groupInfoLayer;
    public GameObject CharactersButtonsLayer => charactersButtonsLayer;
    public GameObject CraftLayer => craftLayerLogic.gameObject;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Init();
        }
    }

    public void Init()
    {
        NothingState = new NothingLayerState(this, interfaceStateMachine);
        CharactersState = new CharactersLayerState(this, interfaceStateMachine);
        GroupState = new GroupLayerState(this, interfaceStateMachine);
        CraftState = new CraftLayerState(this, interfaceStateMachine);
        GameConsoleState = new GameConsoleLayerState(this, interfaceStateMachine);
        FirstCharacterState = new CharacterLayerState(this, interfaceStateMachine, firstPlayerLayer);
        SecondCharacterState = new CharacterLayerState(this, interfaceStateMachine, secondPlayerLayer);
        ThirdCharacterState = new CharacterLayerState(this, interfaceStateMachine, thirdPlayerLayer);
        FourthCharacterState = new CharacterLayerState(this, interfaceStateMachine, fourthPlayerLayer);

        var gMemebers = TheRevenantsAge.GlobalMapController.ChosenGroup.CurrentGroupMembers.ToArray();

        firstPlayerLayer.CurrentCharacter = gMemebers[0];
        secondPlayerLayer.CurrentCharacter = gMemebers.Length > 1 ? gMemebers[1] : null;
        thirdPlayerLayer.CurrentCharacter = gMemebers.Length > 2 ? gMemebers[2] : null;
        fourthPlayerLayer.CurrentCharacter = gMemebers.Length > 3 ? gMemebers[3] : null;
        
        charactersPlateLayerLogic.Init();
        craftLayerLogic.Init();

        Selector.Instance.gameObject.SetActive(true);
        InitializeInterface();
        interfaceStateMachine.Initialize(NothingState, NothingState);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            interfaceStateMachine.ChangeState(NothingState);
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            interfaceStateMachine.ChangeState(GameConsoleState);
        }
    }
    
    private void InitializeInterface()
    {
        MainInfoPanelLayer.SetActive(true);
        GroupButtonsLayer.SetActive(true);
        CharactersButtonsLayer.SetActive(true);
        GroupInfoLayer.SetActive(true);
        CraftLayer.SetActive(true);

        firstPlayerLayer.gameObject.SetActive(true);
        secondPlayerLayer.gameObject.SetActive(true);
        thirdPlayerLayer.gameObject.SetActive(true);
        fourthPlayerLayer.gameObject.SetActive(true);

        MainInfoPanelLayer.SetActive(false);
        GroupButtonsLayer.SetActive(false);
        CharactersButtonsLayer.SetActive(false);
        GroupInfoLayer.SetActive(false);
        CraftLayer.SetActive(false);

        firstPlayerLayer.gameObject.SetActive(false);
        secondPlayerLayer.gameObject.SetActive(false);
        thirdPlayerLayer.gameObject.SetActive(false);
        fourthPlayerLayer.gameObject.SetActive(false);
    }

    public void SetCharactersPanelActive() => interfaceStateMachine.ChangeState(CharactersState);
    public void SetGroupLayerActive() => interfaceStateMachine.ChangeState(GroupState);
    public void ChooseFirstPlayer() => interfaceStateMachine.ChangeState(FirstCharacterState);
    public void ChooseSecondPlayer() => interfaceStateMachine.ChangeState(SecondCharacterState);
    public void ChooseThirdPlayer() => interfaceStateMachine.ChangeState(ThirdCharacterState);
    public void ChooseFourthPlayer() => interfaceStateMachine.ChangeState(FourthCharacterState);
    public void ChooseCraftState() => interfaceStateMachine.ChangeState(CraftState);

    public static void Lock(bool isLock)
    {
        Instance.interfaceStateMachine.IsLock = isLock;
    }
}