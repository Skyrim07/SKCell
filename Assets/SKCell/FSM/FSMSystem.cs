using System.Collections.Generic;

namespace SKCell
{
    /// <typeparam name="TS">State</typeparam>
    /// <typeparam name="TC">Command</typeparam>
    public class FSMSystem<TS, TC>
    {
        protected FSMState<TS, TC> currentState;
        protected Dictionary<TS, FSMState<TS, TC>> states = new Dictionary<TS, FSMState<TS, TC>>();

        public TS CurrentState { get{ return currentState.GetStateID();} }

        public FSMSystem(TS defaultState)
        {
            currentState = State(defaultState);
        }

        public FSMState<TS, TC> State(TS state)
        {
            if (!states.ContainsKey(state))
            {
                FSMState<TS, TC> newState = new FSMState<TS, TC>(state, this);
                states.Add(state, newState);
            }
            return states[state];
        }

        public void ExecuteCommand(TC command)
        {
            var transition = currentState.TryGetTransition(command);
            if (transition != null)
            {
                Transit(transition);
            }
        }

        private void Transit(FSMTransition<TS, TC> transition)
        {
            var from = transition.fromState;
            var to = transition.toState;
            currentState = to;
            from.Exit();
            transition.Transit();
            to.Enter();
        }

        public void Tick()
        {
            currentState?.Update();
            foreach (var transition in currentState.GetAutoTransitions())
            {
                if (currentState.normalizedTime >= transition.normalizedExitTime)
                {
                    Transit(transition);
                    break;
                }
            }
        }

        public void LateTick()
        {
            currentState?.LateUpdate();
        }

        public TS GetCurrentState()
        {
            return currentState.GetStateID();
        }

        public void Dispose()
        {
            currentState.Dispose();
            foreach (var kv in states)
            {
                kv.Value.Dispose();
            }
            states.Clear();
        }
    }
}