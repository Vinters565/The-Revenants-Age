using System.Linq;
using System.Reflection;
using Extension;

namespace Interface.Menu.ForConsole.Commands
{
    public class HelpCommand: ICommand
    {
        public Range ArgsRange { get; } = new (0, 1);
        public string KeyWord => "help";

        public void Action(string[] args)
        {
            if (args.Length == 0)
            {
                var commands = GameConsole.GetCommands()
                    .ToArray();
                var commandsDescr = commands
                    .Select(x => x.GetType())
                    .Select(x => x.GetCustomAttribute<DescriptionForCommandAttribute>())
                    .Select(x => x?.value)
                    .ToArray();

                for (int i = 0; i < commands.Length; i++)
                {
                    var command = commands[i];
                    var commandDescr = commandsDescr[i];
                    
                    if (commandDescr is null)
                        GameConsole.WriteLine($"{command.KeyWord} нет документации");
                    else
                        GameConsole.WriteLine(commandDescr);
                    
                }
            }
            else if (args.Length == 1)
            {
                var commandKeyWord = args[0];
                var command = GameConsole.GetCommands()
                    .FirstOrDefault(a => a.KeyWord == commandKeyWord);
                if (command == null)
                {
                    GameConsole.WriteLine($"Ключевое слово {commandKeyWord} не найдено");
                }
                else
                {
                    var commandDescr = command
                        .GetType()
                        .GetCustomAttribute<DescriptionForCommandAttribute>()
                        ?.value;
                    if (commandDescr == null)
                        GameConsole.WriteLine($"{commandKeyWord} Нет документации");
                    else
                        GameConsole.WriteLine(commandDescr);
                }
            }
        }

        public bool CheckArgs(string[] args)
        {
            return true;
        }
    }
}