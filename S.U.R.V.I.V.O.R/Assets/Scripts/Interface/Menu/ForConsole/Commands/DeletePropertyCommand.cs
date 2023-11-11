using System;
using TheRevenantsAge;
using Range = Extension.Range;

namespace Interface.Menu.ForConsole.Commands
{
    [DescriptionForCommand(
        "deleteProperty <characterFirstName/_> <characterSurname/_> <propertyName> \n" +
        "удаляет свойство с выбранного человека"
    )]
    public class DeletePropertyCommand: ICommand
    {
        public string KeyWord => "deleteProperty";
        public Range ArgsRange { get; } = new (3);
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
                    character.ManBody.Health.DeleteProperty(property);
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