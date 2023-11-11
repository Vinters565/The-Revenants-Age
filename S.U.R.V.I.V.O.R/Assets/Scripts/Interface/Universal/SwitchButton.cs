using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Interface.Universal
{
    [RequireComponent(typeof(Button))]
    public class SwitchButton : MonoBehaviour
    {
        private Button button;
        public event Action<SwitchButton> buttonClicked;

        public UnityEvent layerSetActive;
        public UnityEvent layerSetInactive;

        public Button.ButtonClickedEvent onClick => button.onClick;

        private bool inited;
        
        public void Init(Switch @switch)
        {
            if(inited) return;
            button = GetComponent<Button>();
            button.onClick.AddListener(() => buttonClicked?.Invoke(this));
            @switch.switchedAction += OnSwitchButtonClicked;
            inited = true;
        }

        private void OnSwitchButtonClicked(SwitchButton switchButton)
        {
            if(switchButton == this)
                SetLayerActive();
            else SetLayerInactive();
        }

        protected virtual void SetLayerActive()
        {
            layerSetActive.Invoke();
        }

        protected virtual void SetLayerInactive()
        {
            layerSetInactive.Invoke();
        }
    }
}