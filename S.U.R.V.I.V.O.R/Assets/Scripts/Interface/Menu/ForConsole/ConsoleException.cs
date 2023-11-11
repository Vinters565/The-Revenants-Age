using System;

namespace Interface.Menu.ForConsole
{
    public class ConsoleException: Exception
    {
        public ConsoleException(string massage): base(massage)
        {
        }
    }
}