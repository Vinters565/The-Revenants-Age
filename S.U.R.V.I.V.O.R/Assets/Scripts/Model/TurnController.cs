using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TheRevenantsAge
{
    public class TurnController : MonoBehaviour
    {
        public static TurnController Instance { get; private set; }
        
        [SerializeField] private Button turnEndButton;
        [SerializeField] private UnityEvent turnStartEvent;
        [SerializeField] private UnityEvent turnEndEvent;

        [SerializeField] private List<LineRenderer> turnObj;

        public event Action TurnStarted;
        public event Action TurnEnded;

        public IEnumerable<LineRenderer> TurnObj => turnObj;
    
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                Debug.LogError($"Дублирование {nameof(TurnController)}");
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            turnEndButton.onClick.AddListener(() =>
            {
                turnEndEvent.Invoke();
                TurnEnded?.Invoke();
                
                turnStartEvent.Invoke();
                TurnStarted?.Invoke();
            });
        }
    }
}
