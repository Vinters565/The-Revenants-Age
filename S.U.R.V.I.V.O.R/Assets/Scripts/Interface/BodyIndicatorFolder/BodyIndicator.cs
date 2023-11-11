using System.Collections.Generic;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace Interface.BodyIndicatorFolder
{
    public class BodyIndicator : MonoBehaviour
    {
        [SerializeField] private BodyPartIndicator head;
        [SerializeField] private BodyPartIndicator chest;
        [SerializeField] private BodyPartIndicator stomach;
        [SerializeField] private BodyPartIndicator leftArm;
        [SerializeField] private BodyPartIndicator leftLeg;
        [SerializeField] private BodyPartIndicator rightArm;
        [SerializeField] private BodyPartIndicator rightLeg;
        [SerializeField] private HealthPropertiesDrawer healthPropertiesDrawer;

        private List<Image> allImagesArray;

        private ICharacter character;

        public ICharacter Character
        {
            get => character;
            set
            {
                if(character != null)
                    character.ManBody.Health.HealthPropertiesChanged -= OnHealthPropertiesChanged;
                character = value;
                Init();
                if (character != null)
                    character.ManBody.Health.HealthPropertiesChanged += OnHealthPropertiesChanged;
            }
        }

        private void Init()
        {
            head.Init(character.ManBody.Head);
            chest.Init(character.ManBody.Chest);
            stomach.Init(character.ManBody.Stomach);
            leftArm.Init(character.ManBody.LeftArm);
            rightArm.Init(character.ManBody.RightArm);
            leftLeg.Init(character.ManBody.LeftLeg);
            rightLeg.Init(character.ManBody.RightLeg);
            OnHealthPropertiesChanged();
        }

        private void OnHealthPropertiesChanged()
        {
            if (healthPropertiesDrawer != null && healthPropertiesDrawer.Health != character.ManBody.Health)
            {
                healthPropertiesDrawer.Health = character.ManBody.Health;
            }
        }
    }
}
