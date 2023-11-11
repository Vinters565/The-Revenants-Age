using System;
using System.Collections.Generic;
using System.Linq;
using TheRevenantsAge;

namespace Interface.Menu.ForConsole
{
    public static class CommandsHelper
    {
        private static readonly Dictionary<string, Type> propertyRegister;

        static CommandsHelper()
        {
            propertyRegister = new Dictionary<string, Type>
            (
                Game.GetAllOurType()
                    .Where(x => typeof(IHealthPropertyVisitor).IsAssignableFrom(x))
                    .Select(x => new KeyValuePair<string, Type>(x.Name.ToLower(), x))
            );
        }
        
        public static ICharacter[] GetCharactersBy(string firsName, string surName)
        {
            Func<ICharacter,bool> cond = null;
            if (firsName == "_" && surName == "_")
                cond = x => true;
            else if (firsName == "_")
                cond = x => x.SurName == surName;
            else if (surName == "_")
                cond = x => x.FirstName == firsName;
            else
                cond = x => x.FirstName == firsName && x.SurName == surName;
                
            var characters = 
                TheRevenantsAge.GlobalMapController.Groups
                    .SelectMany(x => x.CurrentGroupMembers)
                    .Where(cond)
                    .ToArray();
            return characters;
        }

        public static IHealthPropertyVisitor GetPropertyBy(string name)
        {
            if (name == null)
                return null;
            
            name = name.ToLower();
            if (!propertyRegister.ContainsKey(name))
                return null;
            return (IHealthPropertyVisitor) Activator.CreateInstance(propertyRegister[name]);
        }
    }
}