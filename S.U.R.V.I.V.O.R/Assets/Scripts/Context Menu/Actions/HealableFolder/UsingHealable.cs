using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Util;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Context_Menu.Actions
{
    [Serializable]
    public class UsingHealable : Healable
    {
        public new void Awake()
        {
            base.Awake();
            ButtonText = "Применить";
        }
        
        [SerializeField] private List<UsingHealableAdditionalProperty> AdditionalProperties;
        public override void InvokeHealMethod(ICharacter character)
        {
            foreach (var additionalProperty in AdditionalProperties)
            {
                if (currentMedicine.CurrentHealingPoints >= additionalProperty.Weight)
                {
                    character.ManBody.Health.AddProperty(Medicine.GetAdditionalPropertyByEnum(additionalProperty.Type));
                    currentMedicine.CurrentHealingPoints -= additionalProperty.Weight;
                }
                else
                {
                    continue;
                }
                if(currentMedicine.CurrentHealingPoints <= 0)
                    Destroy(gameObject);
            }
        }

        public override IEnumerable GetValues()
        {
            var result = new List<Tuple<ICharacter,string>>();
            foreach (var character in TheRevenantsAge.GlobalMapController.ChosenGroup.CurrentGroupMembers)
            {
                result.Add(Tuple.Create((ICharacter)character, $"{character.FirstName} {character.SurName}"));
            }
            return result;
        }
    }

    [Serializable]
    public class UsingHealableAdditionalProperty
    {
        [SerializeField] private Medicine.HealthPropertyToMedicine type;
        [SerializeField] private int weight;

        public Medicine.HealthPropertyToMedicine Type => type;

        public int Weight => weight;
    }
}