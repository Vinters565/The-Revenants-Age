using System;

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TheRevenantsAge
{
    public class SceneTransition : MonoBehaviour
    {
        private static SceneTransition instance;
        public static bool shouldPlayOpeningAnimation = false;

        [Header("Reference")] 
        [SerializeField] private TMP_Text loadingPercent;
        [SerializeField] private Image loadingProgress;

        [Header("Settings")] 
        [SerializeField] [Range(0, 1)] private float lerpSpeed = 0.05f;
        [SerializeField] [Range(0, 1)] private float startProgress;
        [SerializeField] private bool useSlow = true;

        private Animator animator;
        private AsyncOperation asyncOperation;
        private SceneName sceneName;

        private bool isLoading;

        private float nextDrawableProgress;

        private float drawableProgress;
        private float previousDrawableProgress;

        private float startLerpSpeed;
        private float lerpProgress;
        private bool isCompleteAsyncLoad;

        private bool isStartUpdateProgress;

        public static event Action StartLoad;
        public static event Action EndLoad;
        public static event Action StartUnload;
        public static event Action EndUnload;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                animator = GetComponent<Animator>();
            }
        }

        private void Start()
        {
            if (shouldPlayOpeningAnimation)
            {
                animator.SetTrigger("sceneOpening");
                shouldPlayOpeningAnimation = false;
                Debug.Log("StartUnload");
                StartUnload?.Invoke();
            }

            startLerpSpeed = lerpSpeed;
            nextDrawableProgress = startProgress;
        }

        private void Update()
        {
            if (isLoading)
            {
                if (drawableProgress >= 1)
                {
                    OnCompleteLoad();
                    return;
                }

                if (!isCompleteAsyncLoad && asyncOperation.progress >= 0.9f)
                {
                    isCompleteAsyncLoad = true;
                    OnCompletedAsyncLoad();
                }

                if (!isCompleteAsyncLoad && nextDrawableProgress < asyncOperation.progress)
                {
                    OnUpdateAsyncOperationProgress();
                }
            }

            if (isStartUpdateProgress)
            {
                LerpProgress += lerpSpeed;
                drawableProgress = GetLerpProgress(
                    previousDrawableProgress,
                    nextDrawableProgress,
                    LerpProgress);
                UpdateProgress(drawableProgress);
                if (useSlow)
                    lerpSpeed = startLerpSpeed * (1 - LerpProgress);
            }
        }

        private float LerpProgress
        {
            get => lerpProgress;
            set => lerpProgress = Math.Min(1, Math.Max(0, value));
        }

        public static void LoadScene(SceneName sceneName)
        {
            instance.animator.SetTrigger("sceneClosing");
            instance.sceneName = sceneName;
        }


        private float GetLerpProgress(float from, float to, float progress)
        {
            if (progress <= 0)
                return from;
            if (progress >= 1)
                return to;
            var interval = to - from;
            return from + interval * progress;
        }

        private void UpdateProgress(float value)
        {
            loadingPercent.text = $"{Math.Round(value * 100)}%";
            loadingProgress.fillAmount = value;
        }

        private void OnCompleteLoad()
        {
            Debug.Log("EndLoad");
            EndLoad?.Invoke();
            isLoading = false;
            asyncOperation.allowSceneActivation = true;
        }

        private void OnCompletedAsyncLoad()
        {
            LerpProgress = 0;
            lerpSpeed = startLerpSpeed;
            previousDrawableProgress = drawableProgress;
            nextDrawableProgress = 1;
            useSlow = false;
        }

        private void OnUpdateAsyncOperationProgress()
        {
            LerpProgress = 0;
            lerpSpeed = startLerpSpeed;
            previousDrawableProgress = drawableProgress;
            nextDrawableProgress = asyncOperation.progress;
        }

        public void ANIMATION_EVENT_StartAnimation()
        {
            Debug.Log("StartLoad");
            StartLoad?.Invoke();
        }

        public void ANIMATION_EVENT_EndAnimation()
        {
            Debug.Log("EndUnload");
            EndUnload?.Invoke();
        }

        public void ANIMATION_EVENT_StartUpdateProgress()
        {
            isStartUpdateProgress = true;
        }

        public void ANIMATION_EVENT_StartAsyncLoad()
        {
            shouldPlayOpeningAnimation = true;
            isLoading = true;
            asyncOperation = SceneManager.LoadSceneAsync((int) sceneName);
            asyncOperation.allowSceneActivation = false;
        }
    }
}