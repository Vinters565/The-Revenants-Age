using System;
using System.Collections.Generic;
using Interface.ForGameEvents;
using JetBrains.Annotations;
using UnityEngine;

namespace TheRevenantsAge
{
    //Следит только за ивентами, не содержит никакой другой игровой логики(SRP)
    public class GameEventsController : MonoBehaviour
    {
        private class LateEvent
        {
            public readonly IGameEvent gameEvent;
            public uint turnsBeginActivate;

            public LateEvent(IGameEvent gameEvent, uint turnsBeginActivate)
            {
                this.gameEvent = gameEvent;
                this.turnsBeginActivate = turnsBeginActivate;
            }
        }

        public static GameEventsController Instance { get; private set; }
        [SerializeField] private GameEventUI gameEventPrefab;
        [SerializeField] private RectTransform eventsCanvas;
        private readonly List<LateEvent> lateEvents = new();

        public event Action<IGameEvent> EventActivated;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            TurnController.Instance.TurnStarted += OnTurnStart;
        }

        public void ActivateEvent([NotNull] IGameEvent gameEvent)
        {
            if (gameEvent == null) throw new ArgumentNullException(nameof(gameEvent));
            var gameEventObj = Instantiate(gameEventPrefab, eventsCanvas.transform);
            gameEventObj.Initialise(gameEvent);
            EventActivated?.Invoke(gameEvent);
            Debug.Log("Ивент активировался");
        }

        public void AddLateEvent([NotNull] IGameEvent gameEvent, uint turnsBeginActivate)
        {
            if (gameEvent == null) throw new ArgumentNullException(nameof(gameEvent));
            if (turnsBeginActivate == 0)
            {
                ActivateEvent(gameEvent);
            }
            else
            {
                lateEvents.Add(new LateEvent(gameEvent, turnsBeginActivate));
            }
        }

        private void OnTurnStart()
        {
            var removedEvents = new HashSet<LateEvent>();
            foreach (var item in lateEvents)
            {
                if (item.turnsBeginActivate == 0)
                {
                    ActivateEvent(item.gameEvent);
                    removedEvents.Add(item);
                }
                else
                {
                    item.turnsBeginActivate--;
                }
            }

            lateEvents.RemoveAll(x => removedEvents.Contains(x));
        }
    }
}