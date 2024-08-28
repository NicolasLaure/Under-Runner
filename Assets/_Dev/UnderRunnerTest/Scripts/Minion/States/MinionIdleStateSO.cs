using _Dev.UnderRunnerTest.Scripts.FSM;
using UnityEditor.Experimental;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace _Dev.UnderRunnerTest.Scripts.Minion.States
{
    [CreateAssetMenu(fileName = "MinionIdleState", menuName = "Minion/IdleState", order = 0)]
    public class MinionIdleStateSO : MinionStateSO
    {
        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}