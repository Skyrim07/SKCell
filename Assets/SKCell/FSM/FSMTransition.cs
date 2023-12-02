using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    /// <typeparam name="TS">State</typeparam>
    /// <typeparam name="TC">Command</typeparam>
    public class FSMTransition<TS, TC>
    {
        public readonly FSMState<TS,TC> fromState;
        public readonly FSMState<TS, TC> toState;
        public float normalizedExitTime { get; private set; } = 0;
        public bool autoTransit { get; private set; } = false;

        private FSMOnTransitEvt<TS> onTransitEvt;
        private Stack<TC> commandStack = new Stack<TC>();
        private HashSet<FSMCondition> globalConditions = new HashSet<FSMCondition>();
        private Dictionary<TC, HashSet<FSMCondition>> commandConditions = new Dictionary<TC, HashSet<FSMCondition>>();

        public FSMTransition(FSMState<TS, TC> from, FSMState<TS, TC> to)
        {
            this.fromState = from;
            this.toState = to;
        }

        public FSMTransition<TS, TC> ExitTime(float exitTime)
        {
            this.normalizedExitTime = Mathf.Clamp01(exitTime);
            return this;
        }

        public FSMTransition<TS, TC> Auto(bool autoTransit)
        {
            this.autoTransit = autoTransit;
            return this;
        }

        public FSMTransition<TS, TC> On(TC command)
        {
            if (!commandConditions.ContainsKey(command))
            {
                commandConditions.Add(command, new HashSet<FSMCondition>());
            }
            commandStack.Push(command);
            return this;
        }
        public FSMTransition<TS, TC> If(FSMCondition condition)
        {
            if (condition == null)
            {
                SKUtils.EditorLogError("null condition");
                return this;
            }
            
            if (commandStack.Count > 0)
            {
                var cmd = commandStack.Pop();
                commandConditions[cmd].Add(condition);
                commandStack.Clear(); 
            }
            else
            {
                globalConditions.Add(condition);
            }
            return this;
        }

        public FSMTransition<TS, TC> OnTransit(FSMOnTransitEvt<TS> evt)
        {
            onTransitEvt -= evt;
            onTransitEvt += evt;
            return this;
        }

        public void Transit()
        {
            if (onTransitEvt == null)
            {
                return;
            }
            onTransitEvt(fromState.GetStateID(), toState.GetStateID());
        }

        public bool CanTransit(TC command)
        {
            return CommandExists(command) && GlobalConditionsMatched() && CommandConditonMatched(command);
        }

        private bool CommandExists(TC command)
        {
            return commandConditions.ContainsKey(command);
        }

        private bool CommandConditonMatched(TC command)
        {
            if (commandConditions.ContainsKey(command))
            {
                foreach (var cond in commandConditions[command])
                {
                    bool match = cond();
                    if (!match)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return true;
            }
        }

        private bool GlobalConditionsMatched()
        {
            foreach (var cond in globalConditions)
            {
                bool match = cond();
                if (!match)
                {
                    return false;
                }
            }
            return true;
        }

        public void Dispose()
        {
            onTransitEvt = null;
            commandStack.Clear();
            commandConditions.Clear();
            globalConditions.Clear();
        }

        public override int GetHashCode()
        {
            return SKUtils.HashCombine(fromState.GetStateID().GetHashCode(), toState.GetStateID().GetHashCode());
        }

        public override bool Equals(object obj)
        {
            var other = obj as FSMTransition<TS, TC>;
            return other.fromState.Equals(fromState) && other.toState.Equals(this.toState);
        }
    }
}