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

        /// <summary>
        /// transition的触发命令，可以有多个
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public FSMTransition<TS, TC> On(TC command)
        {
            if (!commandConditions.ContainsKey(command))
            {
                commandConditions.Add(command, new HashSet<FSMCondition>());
            }
            commandStack.Push(command);
            return this;
        }

        /// <summary>
        /// transition的条件
        /// 支持全局条件和基于命令的条件
        /// 命令条件必须On和If调用对称：On().If(), 否则就是transition的全局条件
        /// 调用On都会压栈一个command，If取栈顶配对，每次调用If都会清空command的配对stack
        /// 比如希望在A和B状态间连一条transition线，希望任意command下都需要满足条件C（全局条件），希望command1满足条件C1，command2满足条件C2（基于command的条件）,那么：
        /// fsm.State(A).TransitionTo(B).On(command1).If(C1).On(command2).If(C2).If(C).
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public FSMTransition<TS, TC> If(FSMCondition condition)
        {
            if (condition == null)
            {
                CommonUtils.EditorLogError("null condition");
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
            return CommonUtils.HashCombine(fromState.GetStateID().GetHashCode(), toState.GetStateID().GetHashCode());
        }

        public override bool Equals(object obj)
        {
            var other = obj as FSMTransition<TS, TC>;
            return other.fromState.Equals(fromState) && other.toState.Equals(this.toState);
        }
    }
}