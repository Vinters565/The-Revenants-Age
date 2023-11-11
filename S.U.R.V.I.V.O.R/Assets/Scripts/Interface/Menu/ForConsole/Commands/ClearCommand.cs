using Extension;

namespace Interface.Menu.ForConsole.Commands
{
    [DescriptionForCommand(
        "clear [all] очищает консоль\n" +
        "[all] - чистит предыдущие команды"
        )]
    public class ClearCommand: ICommand
    {
        public string KeyWord { get; } = "clear";
        public Range ArgsRange { get; } = new(0 , 1);

        public void Action(string[] args)
        {
            if (args.Length == 0)
                GameConsole.Clear();
            else
            {
                if (args[0] == "all")
                    GameConsole.FullClear();
                else
                    throw new ConsoleException($"Передан неизвестный аргумент {args[0]}");
            }
        }
        public bool CheckArgs(string[] args)
        {
            return true;
        }
    }
}