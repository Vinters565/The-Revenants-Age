using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Audio
{
    public class VolumeController : MonoBehaviour
    {
        [SerializeField] private string exposedName;
    
        private AudioMixer mixer;
        private Slider volumeSlider;
        private Button muteButton;
        private bool isMuted;
        private float currentValue;
        private Sprite muteSprite;
        private Sprite unmuteSprite;

        private void Awake()
        {
            mixer = AudioManager.Instance.Mixer;
        
            volumeSlider = GetComponentInChildren<Slider>();
            muteButton = GetComponentInChildren<Button>();
            volumeSlider.onValueChanged.AddListener(SetVolume);
            muteButton.onClick.AddListener(MuteGroupVolume);
        
            muteSprite = Resources.Load<Sprite>("Interface/Sounds/mute");
            unmuteSprite = Resources.Load<Sprite>("Interface/Sounds/volume");

            var isExist = mixer.GetFloat(exposedName, out var value);
            if (isExist)
                volumeSlider.value = Mathf.Pow(10, value / 20);
        }

        private void SetVolume(float value)
        {
            mixer.SetFloat(exposedName, Mathf.Log10(value) * 20);
            currentValue = value;
        }

        private void MuteGroupVolume()
        {
            if (isMuted)
            {
                mixer.SetFloat(exposedName, Mathf.Log10(currentValue) * 20);
                muteButton.GetComponent<Image>().sprite = unmuteSprite;
            }
            else
            {
                mixer.SetFloat(exposedName, -80.0f);
                muteButton.GetComponent<Image>().sprite = muteSprite;
            }
            isMuted = !isMuted;
        }
    }
}