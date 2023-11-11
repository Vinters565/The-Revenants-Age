using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Interface.PlayerInfoLayer
{
    [System.Serializable]
    public class SerializableSkill
    {
        public SerializableSkill(IconsHelper.Characteristics characteristic, string characteristicName)
        {
            this.characteristic = characteristic;
            this.characteristicName = characteristicName;
        }

        public IconsHelper.Characteristics characteristic;
        public string characteristicName;
    }

    public class SkillInfo : TooltipInstance, ITooltipPart
    {
        [SerializeField] private SegmentProgressBar skillProgressBar;
        [SerializeField] private TextMeshProUGUI skillLevel;
        [SerializeField] private List<(SerializableSkill, float)> characteristics;

        public void Init(int value)
        {
            skillLevel.text = value.ToString();
            skillProgressBar.Init(value);
        }

        public void SetValue(IDrawableSkill skill)
        {
            characteristics = skill.GetLevelInformation();
            skillLevel.text = skill.LevelToDraw.ToString();
            skillProgressBar.SetValue(skill.LevelToDraw);
        }

        public TooltipPartDrawer GetTooltipPart()
        {
            var part = TooltipPartDrawer.InitPart();
            part.AddMainText("Влияние скилла на персонажа");
            if (characteristics is null || characteristics.Count == 0)
            {
                part.AddPlainText("Этот скилл никак не влияет на вашего персонажа");
                return part;
            }
            foreach (var characteristic in characteristics)
            {
                if(characteristic.Item2 == 0) continue;
                part.AddImageWithText($"{characteristic.Item1.characteristicName} {(characteristic.Item2 >= 0 ? "" : "-")}{characteristic.Item2}",IconsHelper.GetCharacteristicIcon(characteristic.Item1.characteristic));
            }
            return part;
        }
    }
}