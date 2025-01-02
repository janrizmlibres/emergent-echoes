using EmergentEchoes.addons.NPC2DNode;
using Godot;
using System;

namespace EmergentEchoes.Utilities.States
{
    public class MoveState : ActionState
    {
        private readonly Node2D _target;

        public MoveState(NPC2D owner, Node2D target) : base(owner)
        {
            _target = target;
        }

        public override void Update()
        {
            OnComplete?.Invoke();
        }

        public event Action OnComplete;
    }
}