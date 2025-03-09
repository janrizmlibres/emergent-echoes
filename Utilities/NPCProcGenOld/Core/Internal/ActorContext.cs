using Godot;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Traits;

namespace NPCProcGen.Core.Internal
{
    public class ActorContext
    {
        public LawfulTrait LawfulModule { get; set; }

        public Sensor Sensor { get; set; }
        public Memorizer Memorizer { get; set; }
        public Executor Executor { get; set; }

        public ActorTag2D Actor { get; private set; }
        public Node2D ActorNode2D { get; private set; }

        public ActorContext(ActorTag2D actor)
        {
            Actor = actor;
            ActorNode2D = actor.GetParent<Node2D>();
        }

        public NPCAgent2D GetNPCAgent2D()
        {
            return Actor as NPCAgent2D;
        }

        public NPCMemorizer GetNPCMemorizer()
        {
            return Memorizer as NPCMemorizer;
        }

        public void EmitSignal(StringName signalName, params Variant[] args)
        {
            Error result = Actor.EmitSignal(signalName, args);
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
        }
    }
}