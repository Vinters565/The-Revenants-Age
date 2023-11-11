using UnityEngine;

namespace Interface.Menu.ForConsole
{
    [CreateAssetMenu(fileName = "New GameConsoleSettings", menuName = "Commands/GameConsoleSettings", order = 51)]
    public class GameConsoleSettings: ScriptableObject
    {
        [Header("AddCommand")]
        [SerializeField] private bool usePostfix;
        [SerializeField] [Min(0)] private int levenshteinDistance;

        public bool UsePostfix => usePostfix;
        public int LevenshteinDistance => levenshteinDistance;
    }
}