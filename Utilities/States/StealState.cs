using EmergentEchoes.addons.NPC2DNode;
using Godot;
using System;

namespace EmergentEchoes.Utilities.States
{
    public class StealState : ActionState
    {
        public StealState(NPC2D owner) : base(owner) { }

        public override void Update()
        {
            OnComplete?.Invoke();
        }

        public event Action OnComplete;
    }
}