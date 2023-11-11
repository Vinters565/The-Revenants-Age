using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Inventory;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Context_Menu.Actions
{
    [RequireComponent(typeof(ContextMenuItem))]
    public abstract class Healable : MonoBehaviour, IContextMenuAction
    {
        public string ButtonText { get; protected set; }

        public bool Extendable { get; private set; }

        public AudioClip SoundOnClick { get; }

        protected Medicine currentMedicine;

        protected InventoryItem item;


        public void Awake()
        {
            Extendable = true;
            currentMedicine = GetComponent<Medicine>();
            item = GetComponent<InventoryItem>();
        }


        public void OnButtonClickAction<T>(T value)
        {
            var character = value as ICharacter;
            InvokeHealMethod(character);
        }

        public abstract void InvokeHealMethod(ICharacter character);

        public virtual IEnumerable GetValues()
        {
            var result = new List<Tuple<ICharacter, string>>();
            foreach (var character in GlobalMapController.ChosenGroup.CurrentGroupMembers)
            {
                if (character.ManBody.Hp < character.ManBody.MaxHp || character.ManBody.BodyParts
                        .Any(x => x.Health.HealthProperties
                            .Any(y =>
                            {
                                if (y is BaseProperty z)
                                {
                                    if (z.BasePropertyType == BasePropertyType.Negative &&
                                        currentMedicine.Data.RemovableHealthProperties
                                            .Any(v =>
                                                Medicine.GetRemovablePropertyByEnum(v.HealthPropertyToMedicine)
                                                    .GetType() == z.GetType()))
                                        return true;
                                }
                                else
                                {
                                    if (currentMedicine.Data.RemovableHealthProperties
                                        .Any(v =>
                                            Medicine.GetRemovablePropertyByEnum(v.HealthPropertyToMedicine)
                                                .GetType() == y.GetType()))
                                        return true;
                                }

                                return false;
                            })))
                {
                    result.Add(Tuple.Create((ICharacter) character, $"{character.FirstName} {character.SurName}"));
                }
            }

            return result;
        }
    }
}