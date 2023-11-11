using Extension;
using UnityEngine;

namespace Interface.Menu.ForConsole
{
    public interface ICommand
    {
        public string KeyWord { get; }
        public Range ArgsRange { get; }
        public void Action(string[] args);
        public bool CheckArgs(string[] args);
    }
}