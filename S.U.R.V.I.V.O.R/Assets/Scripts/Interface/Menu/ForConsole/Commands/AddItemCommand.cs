using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Inventory;
using TheRevenantsAge;
using UnityEngine;
using Range = Extension.Range;

namespace Interface.Menu.ForConsole.Commands
{
    [DescriptionForCommand(
        "add <ItemPrefabName> [count] добавляет предмет в инвентарь локации\n" +
        "[count] - количество предметов, по-умолчанию 1"
    )]
    public class AddItemCommand : ICommand
    {
        private static uint LevenshteinDistanceTolerance => (uint) GameConsole.Settings.LevenshteinDistance;
        private static bool UsePostfix => GameConsole.Settings.UsePostfix;
        public string KeyWord => "add";
        public Range ArgsRange { get; } = new(1, 2);

        public void Action(string[] args)
        {
            //Debug.Log(LevenshteinDistanceTolerance);
            var baseItemName = args[0];

            if (Game.Is2D)
            {
                var postfix = UsePostfix ? Game.PREFAB_2D_POSTFIX : "";
                var resource = AssetsFinder.FindWithLevenshteinDistance(
                        $"{baseItemName}{postfix}.prefab",
                        LevenshteinDistanceTolerance)
                    ?.OfType<GameObject>()
                    .Select(x => x.GetComponent<InventoryItem>())
                    .Where(x => x is not null)
                    .ToList();

                if (resource == null || resource.Count == 0)
                    throw new ConsoleException($"Предмет {baseItemName} был не найден");

                if (resource.Count == 1)
                {
                    if (args.Length == 1)
                    {
                        LocationInventory.InsertItem(UnityEngine.Object.Instantiate(resource[0]));
                    }
                    else 
                    {
                        if (int.TryParse(args[1], out var to))
                        {
                            for (int i = 0; i < to; i++)
                                LocationInventory.InsertItem(UnityEngine.Object.Instantiate(resource[0]));
                        }
                    }

                    GameConsole.WriteLine($"Предмет {baseItemName} был успешно добавлен");
                }
                else
                {
                    GameConsole.WriteLine($"Предмет {baseItemName} был найден в нескольких экземплярах:");
                    foreach (var inventoryItem in resource)
                    {
                        var postfixLen = Game.PREFAB_2D_POSTFIX.Length;
                        var itemName = inventoryItem.name
                            .Remove(inventoryItem.name.Length - postfixLen, postfixLen);
                        GameConsole.WriteLine($"   {itemName}");
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        
        public bool CheckArgs(string[] args)
        {
            if (args.Length == 2 && !int.TryParse(args[1], out _))
            {
                GameConsole.WriteLine($"{args[1]} не число");
                return false;
            }
            return true;
        }
    }
}