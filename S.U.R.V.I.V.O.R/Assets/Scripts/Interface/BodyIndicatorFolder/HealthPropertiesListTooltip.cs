using System.Collections;
using TheRevenantsAge;
using UnityEngine;

namespace Interface.BodyIndicatorFolder
{
    public class HealthPropertiesListTooltip : MonoBehaviour
    {
        [SerializeField] private BasePropertyType typeOfPropertiesToDraw;
        public IEnumerator ShowTooltipCoroutine()
        {
            // yield return new WaitForSeconds(SECONDS);
            // var boostsProperties = GetComponentInParent<PosNegPropertiesDrawer>().Health.HealthProperties
            //     .OfType<BaseProperty>()
            //     .Where(property => property.BasePropertyType == typeOfPropertiesToDraw)
            //     .Select(property => new NestedTooltipItem(property.NameProperty, IconsHelper.GetHealthPropertyIcon(property))).ToList();
            // if (mouseEnter && !ContextMenuController.Instance.IsActive)
            // {
            //     //Tooltip.Instance.ShowTooltip();
            // }
            return default;
        }
    }
}