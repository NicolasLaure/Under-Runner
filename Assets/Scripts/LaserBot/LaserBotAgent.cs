using System.Collections.Generic;
using FSM;
using UnityEngine;

namespace LaserBot
{
    public class LaserBotAgent : Agent
    {
        [Header("Internal Events")]
        [SerializeField] private ActionEventsWrapper entrySequenceEvents;
        [SerializeField] private ActionEventsWrapper moveEvents;
        [SerializeField] private ActionEventsWrapper attackEvents;
        [SerializeField] private ActionEventsWrapper exitSequenceEvents;

        private GameObject player;
        private State _entryState;
        private State _moveState;
        private State _attackState;
        private State _exitState;

        private List<State> _states;
        public Directions dir;

        public GameObject Player
        {
            get { return player; }
            set { player = value; }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Fsm.Enable();
        }

        public void ChangeStateToMove()
        {
            Fsm.ChangeState(_moveState);
        }

        public void ChangeStateToAttack()
        {
            Fsm.ChangeState(_attackState);
        }

        public void ChangeStateToExit()
        {
            Fsm.ChangeState(_exitState);
        }

        protected override List<State> GetStates()
        {
            _entryState = CreateStateWithEvents(entrySequenceEvents);
            _moveState = CreateStateWithEvents(moveEvents);
            _attackState = CreateStateWithEvents(attackEvents);
            _exitState = CreateStateWithEvents(exitSequenceEvents);

            Transition entryToMove = new Transition(_entryState, _moveState);
            _entryState.AddTransition(entryToMove);

            Transition moveToAttack = new Transition(_moveState, _attackState);
            _moveState.AddTransition(moveToAttack);

            Transition attackToExit = new Transition(_attackState, _exitState);
            _attackState.AddTransition(attackToExit);

            _states = new List<State>()
            {
                _entryState,
                _moveState,
                _attackState,
                _exitState
            };

            return new List<State>()
            {
                _entryState,
                _moveState,
                _attackState,
                _exitState
            };
        }

        private State CreateStateWithEvents(ActionEventsWrapper eventsWrapper)
        {
            State state = new State();
            state.EnterAction += eventsWrapper.ExecuteOnEnter;
            state.UpdateAction += eventsWrapper.ExecuteOnUpdate;
            state.ExitAction += eventsWrapper.ExecuteOnExit;

            return state;
        }
    }
}