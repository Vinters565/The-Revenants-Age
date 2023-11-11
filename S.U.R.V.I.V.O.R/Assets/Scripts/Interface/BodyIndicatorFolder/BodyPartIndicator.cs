using TheRevenantsAge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.BodyIndicatorFolder
{
    public class BodyPartIndicator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI bodyPartAmountHp;
        [SerializeField] private HealthPropertiesDrawer healthPropertiesDrawer;
        private BodyPart currentBodyPart;
        
        public void Init(BodyPart bodyPart)
        {
            if (bodyPart == null)
            {
                OnHpChanged(0,0);
                return;
            }
            if (currentBodyPart != null)
                currentBodyPart.HpChanged -= OnHpChanged;
            currentBodyPart = bodyPart;
            OnHpChanged(bodyPart.Hp,bodyPart.Hp);
            currentBodyPart.HpChanged += OnHpChanged;
        }

        private void OnHpChanged(float firstHp, float afterHp)
        {
            if(currentBodyPart == null) return;
            GetComponent<Image>().color = GetColorByNumber(currentBodyPart.Hp / currentBodyPart.MaxHp);
            if(bodyPartAmountHp != null)
                bodyPartAmountHp.text = currentBodyPart.Hp.ToString();
            if (healthPropertiesDrawer != null && healthPropertiesDrawer.Health != currentBodyPart.Health)
                healthPropertiesDrawer.Health = currentBodyPart.Health;
        }

        private static Color32 GetColorByNumber(float number)
        {
            switch (number)
            {
                case 0:
                    return new Color32(0, 0, 0,255);
                case < 0.2f:
                    return new Color32(214, 66, 31,255);
                case < 0.4f:
                    return new Color32(211, 120, 20,255);
                case < 0.6f:
                    return new Color32(219, 185, 0,255);
                case < 0.8f:
                    return new Color32(173, 211, 26,255);
                default:
                    return new Color32(101, 202, 31,255);
            }
        }
    
    }
}