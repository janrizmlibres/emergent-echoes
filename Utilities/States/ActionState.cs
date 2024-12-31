using EmergentEchoes.addons.NPCNode;
using Godot;
using System;

namespace EmergentEchoes.Utilities.States
{
    public abstract class ActionState
    {
        protected readonly NPC2D _owner;

        public ActionState(NPC2D owner)
        {
            _owner = owner;
        }

        public abstract void Update();
    }
}