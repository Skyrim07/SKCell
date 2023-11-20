using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    /// <typeparam name="TS">State</typeparam>
    /// <typeparam name="TC">Command</typeparam>
    public class FSMState<TS, TC>
    {
        readonly FSMSystem<TS, TC> fsm;
        readonly TS stateID;

        public float duration { get; private set; } = 0;
        public float normalizedTime { get; private set; } = 0;
        private FSMOnStateChangeEvt<TS> onEnter;
        private FSMOnStateChangeEvt<TS> onExit;
        private FSMUpdateEvt update;
        private FSMUpdateEvt lateUpdate;
        private Dictionary<FSMTransition<TS, TC>, FSMTransition<TS, TC>> transitions = new Dictionary<FSMTransition<TS, TC>, FSMTransition<TS, TC>>();

        public FSMState(TS id, FSMSystem<TS, TC> fsmSystem)
        {
            stateID = id;
            fsm = fsmSystem;
        }

        public FSMTransition<TS, TC> TransitionTo(TS targetState)
        {
            var from = fsm.State(stateID);
            var to = fsm.State(targetState);
            FSMTransition<TS, TC> transition = new FSMTransition<TS, TC>(from, to);
            if (transitions.ContainsKey(transition))
            {
                return transitions[transition];
            }
            transitions.Add(transition, transition);
            return transition;
        }

        public FSMState<TS, TC> Duration(float duration)
        {
            this.duration = duration;
            return this;
        }

        public FSMTransition<TS, TC> TryGetTransition(TC command)
        {
            foreach (var kv in transitions)
            {
                var transition = kv.Value;
                if (transition.CanTransit(command))
                {
                    return transition;
                }
            }
            return null;
        }

        public FSMTransition<TS, TC>[] GetAutoTransitions()
        {
            List<FSMTransition<TS, TC>> ret = new List<FSMTransition<TS, TC>>();
            foreach (var kv in transitions)
            {
                if (!kv.Value.autoTransit)
                {
                    continue;
                }
                ret.Add(kv.Value);
            }
            return ret.ToArray();
        }

        public FSMState<TS, TC> OnEnter(FSMOnStateChangeEvt<TS> evt)
        {
            normalizedTime = 0;
            onEnter -= evt;
            onEnter += evt;
            return this;
        }

        public FSMState<TS, TC> OnExit(FSMOnStateChangeEvt<TS> evt)
        {
            onExit -= evt;
            onExit += evt;
            return this;
        }

        public FSMState<TS, TC> Update(FSMUpdateEvt evt)
        {
            update -= evt;
            update += evt;
            if (duration > 0)
            {
                normalizedTime += Time.deltaTime / duration;
                normalizedTime = Mathf.Clamp01(normalizedTime);
            }
            else
            {
                normalizedTime = 1;
            }
            return this;
        }

        public FSMState<TS, TC> LateUpdate(FSMUpdateEvt evt)
        {
            lateUpdate -= evt;
            lateUpdate += evt;
            return this;
        }

        public void Update()
        {
            if (update == null)
            {
                return;
            }
            update();
        }

        public void LateUpdate()
        {
            if (lateUpdate == null)
            {
                return;
            }
            lateUpdate();
        }

        public void Exit()
        {
            if (onExit == null)
            {
                return;
            }
            onExit(this.stateID);
        }

        public void Enter()
        {
            if (onEnter == null)
            {
                return;
            }
            onEnter(this.stateID);
        }

        public TS GetStateID()
        {
            return stateID;
        }

        public void Dispose()
        {
            onEnter = null;
            onExit = null;
            update = null;
            lateUpdate = null;
            foreach (var kv in transitions)
            {
                kv.Value.Dispose();
            }
            transitions.Clear();
        }
    }
}