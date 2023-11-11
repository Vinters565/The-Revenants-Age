using System;
using System.Linq;
using TheRevenantsAge;
using Range = Extension.Range;

namespace Interface.Menu.ForConsole.Commands
{
    [DescriptionForCommand(
        "fullHeal восстанавливает здровье всем Character"
    )]
    public class FullHealCommand : ICommand
    {
        public string KeyWord => "fullHeal";
        public Range ArgsRange { get; } = new(0);

        public void Action(string[] args)
        {
            if (Game.Is2D)
            {
                var characters = TheRevenantsAge.GlobalMapController.Groups
                    .SelectMany(x => x.CurrentGroupMembers).ToArray();
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