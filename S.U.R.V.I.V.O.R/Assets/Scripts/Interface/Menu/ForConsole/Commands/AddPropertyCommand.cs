using System;
using System.Linq;
using TheRevenantsAge;
using Range = Extension.Range;

namespace Interface.Menu.ForConsole.Commands
{
    [DescriptionForCommand(
        "addProperty <characterFirstName/_> <characterSurname/_> <propertyName> \n" +
        "добавляет свойство на выбранного человека"
        )]
    public class AddPropertyCommand: ICommand
    {
        public string KeyWord => "addProperty";
        public Range ArgsRange { get; } = new(3);
        public void Action(string[] args)
        {
            var characterName = args[0];
            var characterSurname = args[1];
            var propertyName = args[2];

            var property = CommandsHelper.GetPropertyBy(propertyName);
            if (property == null)
                throw new ConsoleException($"Свойство  {propertyName} не найдено");
            
            if (Game.Is2D)
            {
                var characters = CommandsHelper.GetCharactersBy(characterName, characterSurname);
                foreach (var character in characters)
                    character.ManBody.Health.AddProperty(property);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        
        public bool CheckArgs(string[] args)
        {
            return true;
        }
    }
}