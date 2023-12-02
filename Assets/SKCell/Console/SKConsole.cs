using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace SKCell
{
    [AddComponentMenu("SKCell/Console/SKConsole")]
    public class SKConsole : SKMonoSingleton<SKConsole>
    {
        [SKInspectorText("Please click on < Generate Structure > \nto start using SKConsole for the first time.")]
        [SerializeField] int descrip_text;
        public bool activeOnAwake = false;
        public static bool IsOpen { get { return instance.gameObject.activeSelf; } }
        static SKConsoleCommand root = new SKConsoleCommand();

        [SKFolder("References")]
        [SerializeField] InputField inputField;
        [SerializeField] Transform consoleTipContainer;
        [SerializeField] GameObject consoleTipPF;


        private Stack<SKConsoleCommand> currentCommandStack = new Stack<SKConsoleCommand>();
        public Dictionary<string, SKConsoleCommand> commandLookup;

        protected override void Awake()
        {
            base.Awake();
            inputField.onValueChanged.AddListener(OnInputValueChanged);
            inputField.onEndEdit.AddListener(OnInputComplete);

            currentCommandStack.Push(root);
            commandLookup = new Dictionary<string, SKConsoleCommand>();
            PopulateCommandLookup(root);

            gameObject.SetActive(activeOnAwake);
            if (activeOnAwake)
                Open();
        }

        #region Public Methods

        /// <summary>
        /// Open the console.
        /// </summary>
        public static void Open()
        {
            SKConsole.instance.gameObject.SetActive(true);
            SKConsole.instance.inputField.ActivateInputField();
            SKConsole.instance.OnInputValueChanged(SKConsole.instance.inputField.text);
        }
        /// <summary>
        /// Close the console.
        /// </summary>
        public static void Close()
        {
            SKConsole.instance.gameObject.SetActive(false);
            SKConsole.instance.OnInputValueChanged(SKConsole.instance.inputField.text);
        }
        /// <summary>
        /// Toggle the console. (open console if closed / close console if open)
        /// </summary>
        public static void Toggle()
        {
            if (SKConsole.IsOpen)
                SKConsole.Close();
            else
                SKConsole.Open();
        }

        public static void ActivateInputField()
        {
            SKConsole.instance.inputField.ActivateInputField();
        }

        /// <summary>
        /// Add a command (with no args) to the console.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static SKConsoleCommand AddCommand(string name, string description, Action action = null)
        {
            SKConsoleCommand cmd = new SKConsoleCommand()
            {
                name = name,
                description = description,
                action = action,
                argCount = 0
            };
            root.leafCommands.Add(cmd);
            cmd.parentCommand = root;
            instance.commandLookup[name] = cmd;
            return cmd;
        }
        /// <summary>
        /// Add a command (with 1 arg) to the console.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static SKConsoleCommand AddCommand(string name, string description, Action<float> action)
        {
            SKConsoleCommand cmd = new SKConsoleCommand()
            {
                name = name,
                description = description,
                action1f = action,
                argCount = 1
            };
            root.leafCommands.Add(cmd);
            cmd.parentCommand = root;
            instance.commandLookup[name] = cmd;
            return cmd;
        }
        /// <summary>
        /// Add a command (with 2 args) to the console.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static SKConsoleCommand AddCommand(string name, string description, Action<float, float> action)
        {
            SKConsoleCommand cmd = new SKConsoleCommand()
            {
                name = name,
                description = description,
                action2f = action,
                argCount = 2
            };
            root.leafCommands.Add(cmd);
            cmd.parentCommand = root;
            instance.commandLookup[name] = cmd;
            return cmd;
        }
        /// <summary>
        /// Add a command (with 3 args) to the console.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static SKConsoleCommand AddCommand(string name, string description, Action<float, float, float> action)
        {
            SKConsoleCommand cmd = new SKConsoleCommand()
            {
                name = name,
                description = description,
                action3f = action,
                argCount = 3
            };
            root.leafCommands.Add(cmd);
            cmd.parentCommand = root;
            instance.commandLookup[name] = cmd;
            return cmd;
        }
        #endregion

        #region Private Methods
        private void PopulateCommandLookup(SKConsoleCommand command)
        {
            foreach (var cmd in command.leafCommands)
            {
                commandLookup[cmd.name] = cmd;
                PopulateCommandLookup(cmd);
            }
        }
        private void OnInputValueChanged(string text)
        {
            string[] words = text.Split(' ');
            SKConsoleCommand currentCommand = root;
            currentCommandStack.Clear();
            currentCommandStack.Push(root);

            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (string.IsNullOrEmpty(word) && i != words.Length - 1) continue; 

                SKConsoleCommand nextCommand = FindCommandInCurrentLevel(currentCommand, word);

                if (nextCommand != null)
                {
                    currentCommand = nextCommand;
                    currentCommandStack.Push(nextCommand);
                }
                else if (i < words.Length - 1)
                {
                    break;
                }
            }

            string lastWord = words.Length > 0 ? words[words.Length - 1] : "";
            GenerateTips(currentCommand, lastWord);
        }


        private SKConsoleCommand FindCommandInCurrentLevel(SKConsoleCommand currentCommand, string commandName)
        {
            foreach (var cmd in currentCommand.leafCommands)
            {
                if (cmd.name.Equals(commandName, StringComparison.OrdinalIgnoreCase))
                {
                    return cmd;
                }
            }
            return null;
        }

        private void GenerateTips(SKConsoleCommand currentCommand, string lastWord)
        {
            consoleTipContainer.ClearChildren();

            if (currentCommand != null)
            {
                bool isCommandName = lastWord.Equals(currentCommand.name, StringComparison.OrdinalIgnoreCase);
                bool isArgumentInput = currentCommand.argCount > 0 && (isCommandName || string.IsNullOrWhiteSpace(lastWord) || IsArgument(lastWord));

                if (isArgumentInput || isCommandName)
                {
                    DisplayTip(currentCommand);
                }
                else
                {
                    foreach (var cmd in currentCommand.leafCommands)
                    {
                        if (cmd.name.StartsWith(lastWord, StringComparison.OrdinalIgnoreCase))
                        {
                            DisplayTip(cmd);
                        }
                    }
                }
            }
        }

        private bool IsArgument(string input)
        {
            return float.TryParse(input, out _);
        }

        private void DisplayTip(SKConsoleCommand cmd)
        {
            SKConsoleTip tip = Instantiate(consoleTipPF, consoleTipContainer).GetComponent<SKConsoleTip>();
            string tipText = GetArgText(cmd);
            tip.UpdateInfo(cmd.name, tipText, cmd.description);
        }




        private string GetArgText(SKConsoleCommand cmd)
        {
            if (cmd.leafCommands.Count > 0)
                return "Root Command";
            if (cmd.argCount == 0)
                return "/";
            if (cmd.argCount == 1)
                return "[float]";
            if (cmd.argCount == 2)
                return "[float] [float]";
            if (cmd.argCount == 3)
                return "[float] [float] [float]";
            return "/";
        }

        private void OnInputComplete(string text)
        {
            string[] parts = text.Split(' ');
            if (parts.Length == 0) return;

            SKConsoleCommand command = FindDeepCommand(root, parts);

            if (command != null)
            {
                ExecuteCommand(command, parts);
            }
            else
            {
                SKUtils.EditorLogWarning($"Command not found: {text}");
            }
        }

        private SKConsoleCommand FindDeepCommand(SKConsoleCommand current, string[] parts)
        {
            SKConsoleCommand foundCommand = current;

            foreach (var part in parts)
            {
                bool commandFound = false;
                foreach (var cmd in foundCommand.leafCommands)
                {
                    if (cmd.name.Equals(part, StringComparison.OrdinalIgnoreCase))
                    {
                        foundCommand = cmd;
                        commandFound = true;
                        break;
                    }
                }

                if (!commandFound)
                {
                    if (foundCommand != current)
                    {
                        return foundCommand;
                    }
                    return null;
                }
            }

            return foundCommand;
        }

        private void ExecuteCommand(SKConsoleCommand command, string[] parts)
        {
            if (command == null)
            {
                SKUtils.EditorLogWarning("Command is null.");
                return;
            }

            int commandDepth = CalculateCommandDepth(command);
            int expectedArgCount = parts.Length - commandDepth;
            if (command.argCount == 0 && expectedArgCount == 0)
            {
                command.action?.Invoke();
            }
            else if (command.argCount == 1 && expectedArgCount == 1)
            {
                if (float.TryParse(parts[parts.Length - 1], out float param))
                {
                    command.action1f?.Invoke(param);
                }
                else
                {
                    SKUtils.EditorLogWarning("Invalid argument for command. Expected a float.");
                }
            }
            else if (command.argCount == 2 && expectedArgCount == 2)
            {

                if (float.TryParse(parts[parts.Length - 2], out float param1) &&
                    float.TryParse(parts[parts.Length - 1], out float param2))
                {
                    command.action2f?.Invoke(param1, param2);
                }
                else
                {
                    SKUtils.EditorLogWarning("Invalid arguments for command. Expected two floats.");
                }
            }
            else if (command.argCount == 3 && expectedArgCount == 3)
            {

                if (float.TryParse(parts[parts.Length - 3], out float param1) &&
                    float.TryParse(parts[parts.Length - 2], out float param2) &&
                    float.TryParse(parts[parts.Length - 1], out float param3))
                {
                    command.action3f?.Invoke(param1, param2, param3);
                }
                else
                {
                    SKUtils.EditorLogWarning("Invalid arguments for command. Expected two floats.");
                }
            }
        }

        private int CalculateCommandDepth(SKConsoleCommand command)
        {
            int depth = 0;
            SKConsoleCommand current = command;
            while (current != null && current != root)
            {
                depth++;
                current = current.parentCommand;
            }
            return depth;
        }

        #endregion



#if UNITY_EDITOR

        [SKInspectorButton("Generate Structure")]
        public void GenerateStructure()
        {
            string pathSuffix = "/Console.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SKAssetLibrary.PREFAB_PATH + pathSuffix);
            if (prefab == null)
            {
                SKUtils.EditorLogError("SKConsole Resource Error: prefab lost.");
                return;
            }
            GameObject console = Instantiate(prefab);
            console.name = $"SKConsole";
            console.transform.SetParent(transform.parent);
            console.transform.CopyFrom(transform);
            console.transform.SetSiblingIndex(transform.GetSiblingIndex());
            Selection.activeGameObject = console;
            DestroyImmediate(this.gameObject);
        }
#endif
    }
}
