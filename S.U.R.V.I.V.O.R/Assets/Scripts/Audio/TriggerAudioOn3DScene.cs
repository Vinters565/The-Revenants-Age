using UnityEngine;

namespace Audio
{
    public class TriggerAudioOn3DScene : MonoBehaviour
    {
        [SerializeField] private AudioClip audioClip;
        public AudioClip AudioClip => audioClip;
    }
}
