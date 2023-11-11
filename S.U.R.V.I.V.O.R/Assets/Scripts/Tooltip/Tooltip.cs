using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class Tooltip : MonoBehaviour
    {
        private static Tooltip instance;

        public static Tooltip Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Tooltip>();
                    instance.Init();
                }
                return instance;
            }
        }

        [SerializeField] private float titleWidthLimit = 200;
        [SerializeField] private float widthLimit = 100;
        [SerializeField] private float iconSize = 35f;
        [SerializeField] private RectTransform canvasTransform;
        private RectTransform backgroundRectTransform;
        private List<Action<float>> alphaChangingMethods;

        public RectTransform BackgroundRectTransform
        {
            get => backgroundRectTransform;
            set => backgroundRectTransform = value;
        }

        private Image backgroundImage;
        private RectTransform canvasRectTransform;
        
        private bool coroutineInProgress;
        private bool isShowing;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            if (instance == null)
            {
                instance = this;
                Init();
            }
        }

        private void Init()
        {
            BackgroundRectTransform = transform.Find("Background").GetComponent<RectTransform>();
            backgroundImage = BackgroundRectTransform.GetComponent<Image>();
            canvasRectTransform = canvasTransform;
            alphaChangingMethods = new List<Action<float>>();
            HideTooltip();
        }

        public void Update()
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform,
                Input.mousePosition,
                null,
                out localPoint
            );
            transform.localPosition = localPoint;
            Vector2 anchoredPosition = transform.GetComponent<RectTransform>().anchoredPosition;

            if (anchoredPosition.x < 0)
                anchoredPosition.x = 0;
            else if (anchoredPosition.x + BackgroundRectTransform.rect.width > canvasRectTransform.rect.width)
                anchoredPosition.x = canvasRectTransform.rect.width - BackgroundRectTransform.rect.width;

            if (anchoredPosition.y < 0)
                anchoredPosition.y = 0;
            else if (anchoredPosition.y + BackgroundRectTransform.rect.height > canvasRectTransform.rect.height)
                anchoredPosition.y = canvasRectTransform.rect.height - BackgroundRectTransform.rect.height;


            transform.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        }

        public void ShowTooltip(IEnumerable<ITooltipPart> parts)
        {
            if (coroutineInProgress) return;
            transform.SetAsLastSibling();
            isShowing = true;
            BackgroundRectTransform.GetComponent<LayoutElement>().preferredWidth = widthLimit;
            foreach (var part in parts)
            {
                var tooltipPart = part.GetTooltipPart();
                tooltipPart.name = "TooltipPart";
                tooltipPart.transform.SetParent(BackgroundRectTransform);
            }
            gameObject.SetActive(true);
            foreach (var tmpText in transform.GetComponentsInChildren<TMP_Text>())
            { 
                alphaChangingMethods.Add(x=> tmpText.color = tmpText.color.WithAlpha(x));
            }
            foreach (var image in transform.GetComponentsInChildren<Image>())
            { 
                alphaChangingMethods.Add(x=> image.color = image.color.WithAlpha(x));
            }
            StartCoroutine(SmoothAppearanceCoroutine());
        }   

        private void Clear()
        {
            var parts = BackgroundRectTransform.gameObject.GetComponentsInChildren<RectTransform>(true);
            foreach (var part in parts)
            {
                if(part.name != "Background")
                    Destroy(part.gameObject);
            }
            alphaChangingMethods.Clear();
        }
        
        public void HideTooltip()
        {
            isShowing = coroutineInProgress = false;
            gameObject.SetActive(false);
            alphaChangingMethods = new List<Action<float>>();
            Clear();
        }

        IEnumerator SmoothAppearanceCoroutine()
        {
            coroutineInProgress = true;
            var a = 0f;
            while (a < 1)
            {
                if (!isShowing)
                    break;

                a += Time.deltaTime;
                backgroundImage.color = backgroundImage.color.WithAlpha(a);
                foreach (var alphaChangingMethod in alphaChangingMethods)
                {
                    alphaChangingMethod.Invoke(a);
                }
                yield return null;
            }
            coroutineInProgress = false;
        }
    }
}