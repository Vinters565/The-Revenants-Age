using System;

namespace Interface.Menu.ForConsole
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DescriptionForCommandAttribute: Attribute
    {
        public readonly string value;

        public DescriptionForCommandAttribute(string value)
        {
            this.value = value;
        }
    }
}