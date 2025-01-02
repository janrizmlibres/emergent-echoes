using EmergentEchoes.addons.NPC2DNode;
using EmergentEchoes.Entities.Actors;
using EmergentEchoes.Utilities.States;
using Godot;
using System;

namespace EmergentEchoes.Utilities.Actions
{
    public class TheftAction : NPCAction
    {
        private readonly Actor _target;

        public TheftAction(NPC2D owner, Actor target) : base(owner)
        {
            _currentState = new StealState(_owner);
            _target = target;

            InitializeStates();
        }

        private void InitializeStates()
        {
            MoveState moveState = new(_owner, _target);
            StealState stealState = new(_owner);

            moveState.OnComplete += () => TransitionTo(stealState);
            _currentState = moveState;
        }

        public override void Update()
        {
            _currentState.Update();
        }
    }
}