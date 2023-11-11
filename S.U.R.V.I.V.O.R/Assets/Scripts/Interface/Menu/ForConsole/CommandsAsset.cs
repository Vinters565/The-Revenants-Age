using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interface.Menu.ForConsole
{
    [CreateAssetMenu(fileName = "New CommandsAsset", menuName = "Commands/CommandsAsset", order = 51)]
    public sealed class CommandsAsset: ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<string> commandTypeStrings = new();
        private Dictionary<string, ICommand> commands = new ();
        public IReadOnlyCollection<ICommand> GetCommands() => commands.Values;
        public int Count => commands.Count;
        public bool AddCommand(Type command)
        {
            var commandName = command.FullName;
            if (commandTypeStrings.Contains(commandName))
                return false;
            commandTypeStrings.Add(commandName);
            UpdateCommands();
            return true;
        }

        public void RemoveCommand(Type command)
        {
            var isRemove = commandTypeStrings.Remove(command.FullName);
            if (isRemove)
            {
                UpdateCommands();
            }
        }

        public void CallCommand(string keyWord, string[] args)
        {
            if (!commands.ContainsKey(keyWord))
                throw new ConsoleException("Неизвестная команда");
            var command = commands[keyWord];
            if (command.ArgsRange.End < args.Length || args.Length < command.ArgsRange.Start)
                throw new ConsoleException("Переданно неверное количество параметров");
            if (command.CheckArgs(args))
                command.Action(args);
        }

        public void OnBeforeSerialize()
        {}

        public void OnAfterDeserialize()
        {
            UpdateCommands();
        }

        private void UpdateCommands()
        {
            commandTypeStrings.RemoveAll(x => Type.GetType(x) == null);
            commands = commandTypeStrings
                .Select(x => Type.GetType(x))
                .Select(x => (ICommand)Activator.CreateInstance(x))
                .ToDictionary(x=> x.KeyWord);
        }
    }
}