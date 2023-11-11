using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AYellowpaper.SerializedCollections;
using DataBase;
using Extension;
using Inventory;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.Events;

namespace TheRevenantsAge
{
    [RequireComponent(typeof(GroupMovementLogic), typeof(Pointer))]
    public class Group : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<MoneyType, int> defaultMoney;

        private readonly List<BaseGlobalMapCharacter> currentGroupMembers = new(4);

        private const int MAX_GROUP_MEMBERS = 4;

        public UnityEvent movingOnThisTurnEnded;
        public bool IsLooting { get; set; }
        public GroupMovementLogic GroupMovementLogic { get; private set; }
        public Location Location => GroupMovementLogic.CurrentNode.Location;
        public IReadOnlyList<IGlobalMapCharacter> CurrentGroupMembers => currentGroupMembers;
        public bool IsCanLoot => GroupMovementLogic.IsCanLoot && !IsLooting;
        
        public GroupStatistics GroupStatistics => new (currentGroupMembers.Select(x => x.Statistics).ToArray());

        public int MaxOnGlobalMapGroupEndurance => CurrentGroupMembers.Min(x => x.MaxOnGlobalMapEndurance);
        public int CurrentOnGlobalMapGroupEndurance => CurrentGroupMembers.Min(x => x.CurrentOnGlobalMapEndurance);

        public Currencies Currencies { get; private set; }

        public event UnityAction<Money> MoneyAmountChanged;

        private void Awake()
        {
            GroupMovementLogic = GetComponent<GroupMovementLogic>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var character = transform.GetChild(i).GetComponent<BaseGlobalMapCharacter>();
                if (character != null)
                    currentGroupMembers.Add(character);
            }

            Currencies = new Currencies(defaultMoney);
            Currencies.moneyChanged += money => MoneyAmountChanged.Invoke(money);

            GroupMovementLogic.LocationChanged += OnLocationChanged;
            
            if (currentGroupMembers.Count > MAX_GROUP_MEMBERS)
                throw new ConstraintException();
        }

        public List<T> GetAllGroupItemsByType<T>()
            where T : MonoBehaviour
        {
            var result = new List<T>();
            foreach (var character in CurrentGroupMembers)
            {
                result.AddRange(character.GetItemsFromAllInventoriesByType<T>());
            }
            result.AddRange(LocationInventory.Instance.LocationInventoryGrid.GetItemsFromInventoryByType<T>());
            return result;
        }
        public void Move()
        {
            //TODO Добавить добавление опыта выносливости, силы и так далее
            foreach (var groupMember in CurrentGroupMembers)
            {
                groupMember.ManBody.CurrentOnGlobalMapEndurance -= 1;
            }
        }

        public void EndMovingOnThisTurn()
        {
            //TODO Добавить добавление опыта выносливости, силы и так далее
            foreach (var groupMember in CurrentGroupMembers)
            {
                //TODO Если выносливость и так равна 0, значит чел сходил на максимум, добавить опыт
                groupMember.CurrentOnGlobalMapEndurance = 0;
                movingOnThisTurnEnded.Invoke();
            }
            SubtractEnergy(1);
        }

        public IEnumerable<InventoryItem> Loot(int amountOfTimes)
        {
            return CurrentGroupMembers
                .SelectMany(x => x
                    .Loot(Location.Data, amountOfTimes)
                );
        }
        
        private void SubtractEnergy(int subtractValue)
        {
            foreach (var groupMember in currentGroupMembers)
            {
                groupMember.ManBody.Energy -= subtractValue;
            }
        }

        private void OnLocationChanged(Location obj)
        {
            var @event = obj.Data.GetEvent();
            if (@event != null)
            {
                GameEventsController.Instance.ActivateEvent(@event);
            }
        }
        
        public void OnTurnEnd()
        {
            GroupMovementLogic.OnTurnEnd();
            IsLooting = false;
        }

        public GroupState CreateState()
        {
            var resPath = GetComponent<Pointer>().Address;
            var cgm = CurrentGroupMembers
                .Select(x => x.CreateState())
                .ToArray();
            return new GroupState()
            {
                address = resPath,
                currentGroupMembers = cgm,
                position = transform.position.To2D(),
                isLooting = IsLooting,
                canMove = GroupMovementLogic.CanMove
            };
        }

        public void Restore(GroupState state)
        {
            if (state == null) return;

            foreach (var groupMember in currentGroupMembers)
                Destroy(groupMember. gameObject);
            currentGroupMembers.Clear();

            transform.position = state.position.To3D();
            IsLooting = state.isLooting;

            if (state.currentGroupMembers != null)
            {
                var to = Math.Min(MAX_GROUP_MEMBERS, state.currentGroupMembers.Length);
                for (var i = 0; i < to; i++)
                {
                    var characterState = state.currentGroupMembers[i];
                    var character = characterState.InstantiateGameObj2D();
                    character.transform.SetParent(transform);
                    character.transform.position = Vector3.zero;
                    currentGroupMembers.Add(character.GetComponent<BaseGlobalMapCharacter>());
                }
            }

            GroupMovementLogic.CanMove = state.canMove;
        }
        
        private void OnEnable()
        {
            TurnController.Instance.TurnEnded += OnTurnEnd;
            foreach (var character in currentGroupMembers)
            {
                character.ManBody.Died += () => currentGroupMembers.Remove(character);
            }
        }

        private void OnDisable()
        {
            TurnController.Instance.TurnEnded -= OnTurnEnd;
        }
    }
}