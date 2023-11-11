using System;
using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    public class MusicController : MonoBehaviour
    {
        [SerializeField] private Button nextMusicButton;
        [SerializeField] private Button previousMusicButton;
        [SerializeField] private Button pauseMusicButton;
        [SerializeField] private TextMeshProUGUI nameMusic;
        [SerializeField] private Slider timeSlider;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private Button closeButton;
        [SerializeField] private float transitionDuration = 0.5f;
        
        private AudioManager audioManager;
        private string maxTimeInTimerFormat;
        private bool musicIsPaused;
        private Sprite pauseSprite;
        private Sprite playSprite;
        
        private void Awake()
        {
            audioManager = AudioManager.Instance;
            nextMusicButton.onClick.AddListener(OnNextButtonClick);
            previousMusicButton.onClick.AddListener(OnPreviousButtonClick);
            pauseMusicButton.onClick.AddListener(OnPauseButtonClick);

            timeSlider.onValueChanged.AddListener(SetMusicTime);

            pauseSprite = Resources.Load<Sprite>("Interface/Sounds/pause");
            playSprite = Resources.Load<Sprite>("Interface/Sounds/play");
            
            pauseMusicButton.GetComponent<Image>().sprite = audioManager.MusicIsPaused ? playSprite : pauseSprite;
        }

        private void OnEnable()
        {
            audioManager.WhenMusicChanged += ResetTimeSlider;
            UpdateMusicController();
        }
        
        private void OnDisable()
        {
            audioManager.WhenMusicChanged -= ResetTimeSlider;
        }

        private void Update()
        {
            if (audioManager.CurrentMusic != null)
                timeSlider.value = audioManager.GetMusicTime();
            if (Input.GetMouseButtonDown(0) && !BoundaryCheckMouse(Input.mousePosition, transform) &&
                !BoundaryCheckMouse(Input.mousePosition, closeButton.transform))
                gameObject.SetActive(false);
        }

        public void ShowWindow()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
        
        public void TimeSliderPointerUp()
        {
            if (!musicIsPaused)
            {
                audioManager.UnPauseMusic();
                pauseMusicButton.GetComponent<Image>().sprite = pauseSprite;
            }
            audioManager.SetMusicTime(timeSlider.value);
        }

        public void TimeSliderPointerDown()
        {
            musicIsPaused = audioManager.MusicIsPaused;
            audioManager.PauseMusic();
            pauseMusicButton.GetComponent<Image>().sprite = playSprite;
            audioManager.SetMusicTime(timeSlider.value);
        }
        
        public void TimeSliderDrag()
        {
            audioManager.SetMusicTime(timeSlider.value);
        }

        private void OnNextButtonClick()
        {
            audioManager.NextMusic(transitionDuration);
            ResetTimeSlider(audioManager.CurrentMusic);
        }

        private void OnPreviousButtonClick()
        {
            audioManager.PreviousMusic(transitionDuration);
            ResetTimeSlider(audioManager.CurrentMusic);
        }

        private void OnPauseButtonClick()
        {
            if (audioManager.MusicIsPaused)
            {
                pauseMusicButton.GetComponent<Image>().sprite = pauseSprite;
                audioManager.UnPauseMusic();
            }
            else
            {
                pauseMusicButton.GetComponent<Image>().sprite = playSprite;
                audioManager.PauseMusic();
            }
        }

        private void UpdateMusicController()
        {
            var currentMusic = audioManager.CurrentMusic;
            timeSlider.maxValue = currentMusic.length;
            maxTimeInTimerFormat = ConvertToTimerFormat(timeSlider.maxValue);
            timeSlider.value = audioManager.GetMusicTime();
            timeText.text = $"{ConvertToTimerFormat(timeSlider.value)}/{maxTimeInTimerFormat}";
            nameMusic.text = currentMusic.name;
        }

        private void ResetTimeSlider(AudioClip audioClip)
        {
            timeSlider.value = 0;
            timeSlider.maxValue = audioClip.length;
            nameMusic.text = audioClip.name;
            maxTimeInTimerFormat = ConvertToTimerFormat(timeSlider.maxValue);
            pauseMusicButton.GetComponent<Image>().sprite = pauseSprite;
        }

        private void SetMusicTime(float value)
        {
            timeText.text = $"{ConvertToTimerFormat(value)}/{maxTimeInTimerFormat}";
        }

        private string ConvertToTimerFormat(float time)
        {
            return $"{Mathf.Floor(time / 60):#00}:{Mathf.Floor(time) % 60:00}";
        }

        private bool BoundaryCheckMouse(Vector2 mousePosition, Transform objTransform)
        {
            var position = objTransform.position;
            var rect = objTransform.GetComponent<RectTransform>().rect;
            return ((mousePosition.x > position.x - (rect.width / 2)) &&
                    (mousePosition.x < position.x + (rect.width / 2))) &&
                   ((mousePosition.y < position.y + (rect.height / 2)) &&
                    (mousePosition.y > position.y - (rect.height / 2)));
        }
    }
}