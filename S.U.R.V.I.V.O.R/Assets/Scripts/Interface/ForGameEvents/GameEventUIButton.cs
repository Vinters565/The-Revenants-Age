using System;
using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.ForGameEvents
{
    [RequireComponent(typeof(Button), typeof(InfoTooltip))]
    public class GameEventUIButton : MonoBehaviour
    {
        private Button button;
        private InfoTooltip infoTooltip;

        [SerializeField] private TMP_Text actionName;

        private void Awake()
        {
            infoTooltip = GetComponent<InfoTooltip>();
            button = GetComponent<Button>();
        }

        public void Initialize(IGameEventAction gameEventAction)
        {
            AddDescrToTooltip(gameEventAction);
            actionName.text = gameEventAction.OnButtonText;
            button.onClick.AddListener
            (
                () =>
                {
                    gameEventAction.Rise();
                    Destroy(gameObject.GetComponentInParent<GameEventUI>().gameObject);
                }
            );
        }

        private void AddDescrToTooltip(IGameEventAction gameEventAction)
        {
            infoTooltip.Name = gameEventAction.Name;
            infoTooltip.Description = gameEventAction.Description;
        }
    }
}