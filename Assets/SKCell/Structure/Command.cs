using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    public abstract class Command : ICommand
    {
        /// <summary>
        /// The execute function of this command. Call CommandManager.Do() after calling this function to execute again.
        /// </summary>
        /// <param name="args"></param>
        public virtual void Execute(params float[] args)
        {
            CommandManager.StackPush(this, args);
        }

        /// <summary>
        /// The revert function of this command. Call CommandManager.Undo() to trigger these functions.
        /// </summary>
        /// <param name="args"></param>
        public virtual void Revert(params float[] args)
        {
            
        }
    }
}
