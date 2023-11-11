using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Interface.Universal
{
    public class Switch : MonoBehaviour
    {
        [FormerlySerializedAs("switchButtons")] [SerializeField] private List<SwitchButton> inspectorButtons;

        private List<SwitchButton> switchButtons = new ();

        public event Action<SwitchButton> switchedAction;
        
        [SerializeField] private UnityEvent<SwitchButton> switched;
        
        private void Awake()
        {
            foreach (var switchButton in inspectorButtons)
            {
                AddButton(switchButton);
            }
            
            if (switchButtons.Count > 0)
            {
                switchButtons[0].onClick.Invoke();
            }
        }

        protected void AddButton(SwitchButton button)
        {
            if (switchButtons.Contains(button)) throw new InvalidOperationException();
            button.Init(this);
            button.buttonClicked += OnButtonClick;
            switchButtons.Add(button);
        }
        
        private void OnButtonClick(SwitchButton button)
        {
            switchedAction?.Invoke(button);
            switched.Invoke(button);
        }
    }
}   