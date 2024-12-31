using EmergentEchoes.addons.NPCNode;
using EmergentEchoes.Utilities.States;
using Godot;
using System;

namespace EmergentEchoes.Utilities.Actions
{
    public abstract class NPCAction
    {
        protected readonly NPC2D _owner;
        protected ActionState _currentState;

        public NPCAction(NPC2D owner)
        {
            _owner = owner;
        }

        protected void TransitionTo(ActionState newState)
        {
            _currentState = newState;
        }

        public abstract void Update();
    }
}