using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    public class ButtonTriggerAudio : MonoBehaviour
    {
        [field: SerializeField] private AudioClip audioClip;

        private void Reset()
        {
            audioClip = audioClip ? audioClip : Sounds.GetUISoundEffect(UISoundEffectsType.DefaultButton);
        }

        private void Start()
        {
            audioClip = audioClip ? audioClip : Sounds.GetUISoundEffect(UISoundEffectsType.DefaultButton);
            GetComponent<Button>().onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            AudioManager.Instance.PlayOneShotSFX(audioClip);
        }
    }
}