using System.Linq;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.BodyIndicatorFolder
{
    public class PosNegPropertiesDrawer : HealthPropertiesDrawer
    {
        [SerializeField] private LayoutGroup negativeIconsLayoutGroup;
        protected override void DeleteAllHealthProperties()
        {
            var comps = allOrPositiveIconsLayoutGroup.GetComponentsInChildren<Image>()
                .Concat(negativeIconsLayoutGroup.GetComponentsInChildren<Image>());
            foreach (var image in comps)
            {
                Destroy(image.gameObject);
            }
        }

        protected override void DrawAllHealthProperties()
        {
            foreach (var healthProperty in health.HealthProperties.OfType<BaseProperty>())
            {
                var parentRect = allOrPositiveIconsLayoutGroup.GetComponent<RectTransform>();
  
                parentRect = healthProperty.BasePropertyType == BasePropertyType.Negative
                    ? negativeIconsLayoutGroup.GetComponent<RectTransform>()
                    : parentRect;
                
            
                var icon = Instantiate(iconPrefab, parentRect, false);
                icon.GetComponent<Image>().sprite = IconsHelper.GetHealthPropertyIcon(healthProperty);
                icon.GetComponent<RectTransform>().sizeDelta = sizeOfImage;
                //icon.GetComponent<HealthPropertyTooltip>().property = healthProperty;
            }
        }
    }
}
