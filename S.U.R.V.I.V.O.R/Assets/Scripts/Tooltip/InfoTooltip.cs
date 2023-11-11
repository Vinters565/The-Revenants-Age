using UnityEngine;

namespace Interface
{
    public class InfoTooltip : TooltipInstance, ITooltipPart
    {
        [SerializeField] private new string name;
        [SerializeField] private string description;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public TooltipPartDrawer GetTooltipPart()
        {
            var part = TooltipPartDrawer.InitPart();
            part.AddMainText(name);
            part.AddPlainText(description);
            return part;
        }
    }
}