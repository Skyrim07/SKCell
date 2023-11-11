using System;
using System.Collections.Generic;

namespace SKCell
{
    public class SKConsoleCommand
    {
        public string name;
        public string description;  
        public Action action;
        public Action<float> action1f;
        public Action<float, float> action2f;
        public Action<float, float, float> action3f;
        public int argCount = 0;

        public List<SKConsoleCommand> leafCommands = new List<SKConsoleCommand>();
        public SKConsoleCommand parentCommand;

        public SKConsoleCommand AddCommand(SKConsoleCommand cmd)
        {
            leafCommands.Add(cmd);
            return cmd;
        }

        /// <summary>
        /// Add a sub-command (with no args) to this command.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public SKConsoleCommand AddCommand(string name, string description, Action action = null)
        {
            SKConsoleCommand cmd = new SKConsoleCommand()
            {
                name = name,
                description = description,
                action = action,
                argCount = 0
            };
            leafCommands.Add(cmd);
            cmd.parentCommand = this;
            SKConsole.instance.commandLookup[name] = cmd;
            return cmd;
        }

        /// <summary>
        /// Add a sub-command (with 1 arg) to this command.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public SKConsoleCommand AddCommand(string name, string description, Action<float> action)
        {
            SKConsoleCommand cmd = new SKConsoleCommand()
            {
                name = name,
                description = description,
                action1f = action,
                argCount = 1
            };
            leafCommands.Add(cmd);
            cmd.parentCommand = this;
            SKConsole.instance.commandLookup[name] = cmd;
            return cmd;
        }

        /// <summary>
        /// Add a sub-command (with 2 args) to this command.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public SKConsoleCommand AddCommand(string name, string description, Action<float, float> action)
        {
            SKConsoleCommand cmd = new SKConsoleCommand()
            {
                name = name,
                description = description,
                action2f = action,
                argCount = 2 
            };
            leafCommands.Add(cmd);
            cmd.parentCommand = this;
            SKConsole.instance.commandLookup[name] = cmd;
            return cmd;
        }

        /// <summary>
        /// Add a sub-command (with 3 args) to this command.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public SKConsoleCommand AddCommand(string name, string description, Action<float, float, float> action)
        {
            SKConsoleCommand cmd = new SKConsoleCommand()
            {
                name = name,
                description = description,
                action3f = action,
                argCount = 3
            };
            leafCommands.Add(cmd);
            cmd.parentCommand = this;
            SKConsole.instance.commandLookup[name] = cmd;
            return cmd;
        }
    }
}
