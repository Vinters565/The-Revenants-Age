using System.Linq;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.BodyIndicatorFolder
{
    public class HealthPropertiesDrawer : MonoBehaviour
    {
        [SerializeField] protected PropertyManager health;
        [SerializeField] protected LayoutGroup allOrPositiveIconsLayoutGroup;
        [SerializeField] protected GameObject iconPrefab;
        [SerializeField] protected Vector2 sizeOfImage;
    
        public PropertyManager Health
        {
            get => health;
            set
            {
                var prevHealth = health;
                health = value;
                OnHealthChanged(prevHealth);
                ReDraw();
            }
        }

        private void OnHealthChanged(PropertyManager previousHealth)
        {
            if(previousHealth is not null)
                previousHealth.HealthPropertiesChanged -= ReDraw;
            health.HealthPropertiesChanged += ReDraw;
        }

        private void ReDraw()
        {
            DeleteAllHealthProperties();
            DrawAllHealthProperties();
        }

        protected virtual void DeleteAllHealthProperties()
        {
            var comps = allOrPositiveIconsLayoutGroup.GetComponentsInChildren<Image>();
            foreach (var image in comps)
            {
                Destroy(image.gameObject);
            }
        }
    
        protected virtual void DrawAllHealthProperties()
        {
            foreach (var healthProperty in health.HealthProperties.OfType<BaseProperty>())
            {
                var icon = Instantiate(iconPrefab,allOrPositiveIconsLayoutGroup.GetComponent<RectTransform>(),false);
                icon.GetComponent<Image>().sprite = IconsHelper.GetHealthPropertyIcon(healthProperty);
                icon.GetComponent<RectTransform>().sizeDelta = sizeOfImage;
                //icon.GetComponent<HealthPropertyTooltip>().property = healthProperty;
            }
        }
        
        private void OnEnable()
        {
            if (health == null) return;
            health.HealthPropertiesChanged += ReDraw;
            ReDraw();
        }

        private void OnDisable()
        {
            if (health == null) return;
            health.HealthPropertiesChanged -= ReDraw;
        }
    }
}
