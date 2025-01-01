using EmergentEchoes.Utilities.Actions;
using Godot;
using System;

namespace EmergentEchoes.Utilities.Internal
{
    public class Executor
    {
        private readonly NPCAction _action;

        public Executor(NPCAction action)
        {
            _action = action;
        }

        public void Update()
        {
            _action.Update();
        }
    }
}