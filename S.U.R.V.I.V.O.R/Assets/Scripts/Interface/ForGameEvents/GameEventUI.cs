using System;
using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.ForGameEvents
{
    public class GameEventUI: MonoBehaviour
    {
        [SerializeField] private TMP_Text eventName;
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text description;
        [SerializeField] private LayoutGroup actions;

        [SerializeField] private GameEventUIButton actionButtonPrefab;
        
        public void Initialise(IGameEvent gameEvent)
        {
            if (gameEvent == null)
                throw new ArgumentException();

            eventName.text = gameEvent.Name;
            image.sprite = gameEvent.Image;
            description.text = gameEvent.Description;

            foreach (var gameEventAction in gameEvent.Actions)
            {
                var button = Instantiate(actionButtonPrefab, actions.transform);
                button.Initialize(gameEventAction);
            }
        }
    }
}