using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheRevenantsAge;
using TMPro;
using UnityEngine;

namespace Interface.Menu.ForConsole
{
    public sealed class GameConsole : MonoBehaviour
    {
        public static GameConsole Instance { get; private set; }

        private const int MAX_PREVIOUS_COMMANDS_COUNT = 30;

        [SerializeField] private GameConsoleSettings settings;
        [SerializeField] private CommandsAsset commands;

        [Header("")] [SerializeField] private TextMeshProUGUI commandTextPrefab;
        [SerializeField] private GameObject container;
        [SerializeField] private ScrollController scroll;
        [SerializeField] private TMP_InputField inputField;


        private List<string> previousCommands;

        private int cycleIndex = 0;

        private string dataPath;

        private bool isReadKey;

        public static IReadOnlyCollection<ICommand> GetCommands() => Instance?.commands.GetCommands();
        public static GameConsoleSettings Settings => Instance?.settings;

        private int CycleIndex
        {
            get => cycleIndex;
            set
            {
                if (value < 0)
                    cycleIndex = value % (previousCommands.Count + 1) + previousCommands.Count;
                else
                    cycleIndex = value % previousCommands.Count;
            }
        }

        private string CurrentPreviousCommand => previousCommands[cycleIndex];

        public static bool IsActive => Instance.gameObject.activeSelf;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                Init();
            }
        }

        private void Init()
        {
            if (!CheckValid())
            {
                gameObject.SetActive(false);
                return;
            }

            dataPath = Path.Join(Application.temporaryCachePath, "ConsoleCash.xml");
            gameObject.SetActive(false);
            container.SetActive(true);
            if (!File.Exists(dataPath))
                previousCommands = new List<string> {""};
            else
            {
                previousCommands = Serializer.ReadObject<List<string>>(dataPath);
                ResetCycleIndex();
            }
        }


        private void Update()
        {
            if (!isReadKey)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    OnCommandEntered();
                }
                else if (previousCommands.Count != 0)
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        CycleIndex--;
                        inputField.text = CurrentPreviousCommand;
                        FocusToInput();
                    }
                    else if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        CycleIndex++;
                        inputField.text = CurrentPreviousCommand;
                        FocusToInput();
                    }
                }
            }
        }

        private void OnEnable()
        {
            FocusToInput();
        }

        private void OnDisable()
        {
        }

        private bool CheckValid()
        {
            if (commands == null)
            {
                Debug.LogError($"Поле {nameof(commands)} не проинициализировано!");
                return false;
            }

            if (settings == null)
            {
                Debug.LogError($"Поле {nameof(settings)} не проинициализировано!");
                return false;
            }

            return true;
        }

        private void OnCommandEntered()
        {
            var command = inputField.text;
            if (string.IsNullOrEmpty(command))
                return;
            AddToPreviousCommands(command);
            var tempSplit = command.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            try
            {
                inputField.text = "";
                commands.CallCommand(tempSplit[0], tempSplit.Skip(1).ToArray());
            }
            catch (ConsoleException e)
            {
                AddElementToScrollEnd(e.Message);
            }
        }

        private void FocusToInput()
        {
            inputField.ActivateInputField();
            inputField.Select();
        }

        private void AddToPreviousCommands(string command)
        {
            previousCommands.Remove(command);
            if (previousCommands.Count == MAX_PREVIOUS_COMMANDS_COUNT)
            {
                previousCommands.RemoveAt(0);
            }

            previousCommands[^1] = command;
            previousCommands.Add("");

            ResetCycleIndex();
        }

        private void ResetCycleIndex()
        {
            CycleIndex = previousCommands.Count - 1;
        }

        private void AddElementToScrollEnd(string str)
        {
            var text = Instantiate(commandTextPrefab);
            text.text = str;
            scroll.AddElementEnd(text.rectTransform);
        }

        public static void Clear()
        {
            Instance.scroll.Clear();
        }

        public static void FullClear()
        {
            Clear();
            Instance.cycleIndex = 0;
            Instance.previousCommands = new List<string>() {""};
        }

        public static void WriteLine(string str)
        {
            Instance.AddElementToScrollEnd(str);
        }

        public static void ReadKey(Action<string> onReading)
        {
            Instance.StartCoroutine(ReadKeyCoroutine(onReading));
        }

        private static IEnumerator ReadKeyCoroutine(Action<string> onReading)
        {
            Instance.FocusToInput();
            Instance.isReadKey = true;

            while (true)
            {
                yield return null;
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    onReading?.Invoke(Instance.inputField.text);
                    Instance.inputField.text = "";
                    Instance.isReadKey = false;
                    yield break;
                }
            }
        }

        private void OnDestroy()
        {
            Serializer.WriteObject(dataPath, previousCommands);
        }
    }
}