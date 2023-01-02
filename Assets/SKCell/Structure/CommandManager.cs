using System;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    public static class CommandManager
    {
        public static Stack<(ICommand, float[])> cmdStack = new Stack<(ICommand, float[])>();
        public static int cmdStackCapacity = 100;
        public static int cmdStackStartCount = 10;

        public static (ICommand, float[]) LastCommand 
        { 
            get 
            {
                return cmdStack.Peek(); 
            } 
        }

        /// <summary>
        /// Execute the last command.
        /// </summary>
        public static void Do()
        {
            if (cmdStack.Count == 0)
                return;

            (ICommand, float[]) doCmd = cmdStack.Peek();
            doCmd.Item1.Execute(doCmd.Item2);

            if (cmdStack.Count > cmdStackCapacity)
            {
                RotateStack();
            }
        }

        /// <summary>
        /// Undo a certain number of commands.
        /// </summary>
        /// <param name="count"></param>
        public static void Undo(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                if (cmdStack.Count > 0)
                {
                    (ICommand, float[]) doCmd = cmdStack.Pop();
                    doCmd.Item1.Revert(doCmd.Item2);
                }
            }
        }

        private static void RotateStack()
        {
            List<(ICommand, float[])> cmdList = new List<(ICommand, float[])>();
            for (int i = 0; i < cmdStackStartCount; i++)
            {
                cmdList.Add(cmdStack.Pop());
            }

            cmdStack = new Stack<(ICommand, float[])>();
            for (int i = cmdList.Count - 1; i >= 0; i--)
            {
                cmdStack.Push(cmdList[i]);
            }
        }

        /// <summary>
        /// Push a command into the stack.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        public static void StackPush(ICommand cmd, float[] args)
        {
            cmdStack.Push((cmd, args));
        }
    }
}
