using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Interface
{
    [RequireComponent(typeof(Canvas))]
    public class MainCanvas : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static MainCanvas Instance;

        private Canvas canvas;

        public Canvas Canvas => canvas;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogError("Global Map Canvas is not singleton");
            }

            canvas = GetComponent<Canvas>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Selector.Deactivate();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Selector.Activate();
        }
    }
}
