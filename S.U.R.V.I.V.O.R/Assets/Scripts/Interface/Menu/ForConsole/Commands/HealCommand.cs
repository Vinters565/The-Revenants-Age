using System;
using System.Linq;
using TheRevenantsAge;
using Range = Extension.Range;

namespace Interface.Menu.ForConsole.Commands
{
    [DescriptionForCommand(
        "heal <characterFirstName> <characterSurname>\n"+
        "Полностью исцеляет здоровье всех персонажей с таким именем и фамилией"
        )]
    public class HealCommand : ICommand
    {
        public string KeyWord => "heal";
        public Range ArgsRange { get; } = new (2);

        public void Action(string[] args)
        {
            var characterName = args[0];
            var characterSurname = args[1];
            if (Game.Is2D)
            {
                var characters = CommandsHelper.GetCharactersBy(characterName, characterSurname);
                if (characters.Length == 0)
                    throw new ConsoleException($"{characterName} {characterSurname} не был найден");

                foreach (var character in characters)
                    character.ManBody.FullHeal();
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