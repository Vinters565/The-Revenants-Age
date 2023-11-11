using TheRevenantsAge;
using UnityEngine;

namespace Audio
{
    public class EntityAudioManager : MonoBehaviour
    {
        [SerializeField] private AudioClip attackedAudioClip;
        [SerializeField] private AudioClip takingDamageAudioClip;
    
        private IFightCharacter entity;

        private void Awake()
        {
            entity = GetComponent<IFightCharacter>();
        }

        private void OnEnable()
        {
            entity.WhenDamagedOrHealed += PlaySoundWhenTakingDamage;
            //entity.WhenDefaultAttacked.AddListener(PlaySoundWhenAttacked);
        }

        private void OnDisable()
        {
            entity.WhenDamagedOrHealed -= PlaySoundWhenTakingDamage;
            //entity.WhenDefaultAttacked.RemoveListener(PlaySoundWhenAttacked);
        }

        private void PlaySoundWhenTakingDamage(float f, Vector3 vector3)
        {
            if (f < 0)
                AudioManager.Instance.PlaySFX(takingDamageAudioClip);
        }
    
        private void PlaySoundWhenAttacked() => AudioManager.Instance.PlaySFX(attackedAudioClip);
    
    }
}