using System;
using System.Collections.Generic;
using System.Linq;
using Interface;
using Inventory;
using UnityEngine;

namespace TheRevenantsAge
{
    public class GlobalMapController : MonoBehaviour, ITurnEndAction
    {
        public static GlobalMapController Instance { get; private set; }
        private static GlobalMapState state;
        
        private static int mainGroupIndex;
        private static int mainCharacterIndex;
        private static int chosenGroupIndex;

        public static bool OnPause { get; private set; }
        public static int TurnNumber { get; private set; } = 1;

        [SerializeField] private List<Group> groups;
        [field: SerializeField] public Canvas MainCanvas { get; private set; }


        public static GlobalMapState State => state;
        public static int ChosenGroupIndex
        {
            get => chosenGroupIndex;
            set
            {
                if (value < 0 || value >= Instance.groups.Count)
                    throw new InvalidOperationException();
                ChosenGroupChange?.Invoke(Instance.groups[chosenGroupIndex], Instance.groups[value]);
                chosenGroupIndex = value;
            }
        }

        public static IGlobalMapCharacter MainCharacter => Groups[mainGroupIndex].CurrentGroupMembers[mainCharacterIndex];
        public static Group MainGroup => Groups[mainCharacterIndex];
        public static Group ChosenGroup => Instance.groups[chosenGroupIndex];
        public static IReadOnlyList<Group> Groups => Instance.groups;
        
        /// <summary>
        /// ОБЯЗАТЕЛЬНА ОТПИСКА
        /// </summary>
        public static event Action<Group, Group> ChosenGroupChange;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }
        

        public void Start()
        {
            if (GameSession.IsLoadedInGlobalMap)
            {
                GameSession.Current= new GameSession("Debug");
            }
            else
            {
                Clear();
                Restore(State);
            }

            if (!GameSession.Current.SaveExists("Initial"))
                GameSession.Current.Save("Initial");
        }

        private void OnEnable()
        {
            MainCharacter.ManBody.Died += OnGameEnd;
            TurnController.Instance.TurnEnded += OnTurnEnd;
        }

        private void OnDisable()
        {
            MainCharacter.ManBody.Died -= OnGameEnd;
            TurnController.Instance.TurnEnded -= OnTurnEnd;
        }

        private void OnDestroy()
        {
            ChosenGroupChange = null;
        }

        public void Resume()
        {
            OnPause = false;
            Selector.DeactivateImmediate();
            MinimapController.Instance.isActive = true;
            CameraController.Instance.IsActive = true;
            InventoryController.Instance.gameObject.SetActive(true);
        }

        public void Pause()
        {
            OnPause = true;
            Tooltip.Instance.HideTooltip();
            Selector.Activate();
            MinimapController.Instance.isActive = false;
            CameraController.Instance.IsActive = false;
            InventoryController.Instance.gameObject.SetActive(false);
        }

        private static void OnGameEnd()
        {
            GameStatistics.groupDeadStatistics = MainGroup.GroupStatistics;
            GameStatistics.characterDeadStatistics = MainCharacter.Statistics;
            MainCharacter.ManBody.Died -= OnGameEnd;
            SceneTransition.LoadScene(SceneName.DeadScene);
        }
        public void OnTurnEnd() => TurnNumber++;
        public static void UpdateState() => state = Instance.CreateState();

        public static void ClearState() => state = null;

        public static void SetState(GlobalMapState newState) => state = newState;

        public GlobalMapState CreateState()
        {
            return new GlobalMapState()
            {
                mainCharacterIndex = mainCharacterIndex,
                mainGroupIndex = mainGroupIndex,
                turnNumber = TurnNumber,
                groups = groups.Select(g => g.CreateState()).ToList(),
                chosenGroupIndex = ChosenGroupIndex,
                locationInventory = LocationInventory.Instance.LocationInventoryGrid
                    .GetItems()
                    .Select(x => x
                        .GetComponent<BaseItem>()
                        .CreateState())
                    .ToArray(),
                cameraPosition = CameraController.Instance.transform.position,
                zoomHeight = CameraController.Instance.zoomHeight
            };
        }

        public void Restore(GlobalMapState state)
        {
            if (state == null) return;

            mainCharacterIndex = state.mainCharacterIndex;
            mainGroupIndex = state.mainGroupIndex;

            groups = new List<Group>();
            if (state.groups != null)
            {
                foreach (var groupState in state.groups)
                {
                    var group = groupState.InstantiateGameObj();
                    groups.Add(group);
                }
            }

            ChosenGroupIndex = state.chosenGroupIndex;
            TurnNumber = state.turnNumber;

            if (state.locationInventory != null)
            {
                var inventory = LocationInventory.Instance.LocationInventoryGrid;
                foreach (var itemState in state.locationInventory)
                {
                    var item = itemState.InstantiateGameObj2D();
                    if (item is null) continue;
                    var invItem = item.GetComponent<InventoryItem>();
                    inventory.PlaceItem(invItem, invItem.OnGridPositionX, invItem.OnGridPositionY);
                }
            }

            CameraController.Instance.MoveCamera(state.cameraPosition);
            CameraController.Instance.zoomHeight = state.zoomHeight;
            InterfaceController.Instance.Init();
        }

        private void Clear()
        {
            foreach (var group in groups)
                Destroy(group.gameObject);

            groups = new List<Group>();
            foreach (var item in FindObjectsOfType<InventoryItem>(true))
                Destroy(item.gameObject);
        }

        // IEnumerator RestoreGameCoroutine()
        // {
        //     yield return null;
        //     Clear();
        //     yield return null;
        //     Restore(State);
        // }

        // IEnumerator CreateInitialGameCoroutine()
        // {
        //     yield return null;
        //     if (!GameSession.Current.SaveExists("Initial"))
        //         GameSession.Current.Save("Initial");
        // }
    }
}