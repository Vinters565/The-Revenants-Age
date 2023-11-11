using TheRevenantsAge;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(Weapon))]
    public class WeaponAudioManager : MonoBehaviour
    {
        [SerializeField]private AudioClip audioClip;
    
        private Weapon weapon;
    
        private void Awake()
        {
            weapon = GetComponent<Weapon>();
        }

        private void OnEnable()
        {
            weapon.WhenAttacked.AddListener(PlaySound);
        }

        private void OnDisable()
        {
            weapon.WhenAttacked.RemoveListener(PlaySound);
        }

        private void PlaySound() => AudioManager.Instance.PlayOneShotSFX(audioClip);
    
    }
}
