using Godot;
using System.Collections.Generic;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Helpers;
using System.Linq;
using NPCProcGen.Core.Components.Enums;

namespace NPCProcGen.Core.Internal
{
    public class Memorizer
    {
        protected readonly ActorTag2D _owner;

        protected readonly Dictionary<ActorTag2D, ActorData> _actorData = new();

        public Memorizer(ActorTag2D owner)
        {
            _owner = owner;
        }

        public virtual void Initialize(List<ActorTag2D> actors)
        {
            foreach (ActorTag2D actor in actors)
            {
                _actorData.Add(actor, new ActorData(actor));
            }
        }

        public virtual void Update(double delta)
        {
            foreach (ActorData actorData in _actorData.Values)
            {
                actorData.Update(delta);
            }
        }

        public float GetActorRelationship(ActorTag2D actor) => _actorData[actor].Relationship;

        public void UpdateRelationship(ActorTag2D actor, float amount)
        {
            DebugTool.Assert(_actorData.ContainsKey(actor), $"Actor {actor.Parent.Name} not found in memorizer.");
            _actorData[actor].Relationship += amount;
        }

        public List<ActorTag2D> GetPeerActors() => _actorData.Keys.ToList();
        public bool IsFriendly(ActorTag2D actor) => _actorData[actor].IsFriendly();
        public bool IsTrusted(ActorTag2D actor) => _actorData[actor].IsTrusted();
        public bool IsClose(ActorTag2D actor) => _actorData[actor].IsClose();

        public virtual Vector2? GetLastKnownPosition(ActorTag2D actor) => null;
        public virtual void UpdateLastKnownPosition(ActorTag2D actor, Vector2 location) { }
        public virtual void UpdateLastPetitionResource(ActorTag2D actor, ResourceType type) { }
        public virtual bool IsValidPetitionTarget(ActorTag2D actor, ResourceType type) => false;
    }

    public class NPCMemorizer : Memorizer
    {
        public NPCMemorizer(NPCAgent2D owner) : base(owner) { }

        public override void Initialize(List<ActorTag2D> actors)
        {
            foreach (ActorTag2D actor in actors)
            {
                _actorData.Add(actor, new NPCActorData(actor));
            }
        }

        public override Vector2? GetLastKnownPosition(ActorTag2D actor)
        {
            DebugTool.Assert(_actorData[actor] is NPCActorData, "Actor data is not of type NPCActorData.");
            return (_actorData[actor] as NPCActorData).LastKnownPosition;
        }

        public override void UpdateLastKnownPosition(ActorTag2D actor, Vector2 location)
        {
            DebugTool.Assert(_actorData[actor] is NPCActorData, "Actor data is not of type NPCActorData.");
            (_actorData[actor] as NPCActorData).LastKnownPosition = location;
        }

        public override void UpdateLastPetitionResource(ActorTag2D actor, ResourceType type)
        {
            DebugTool.Assert(_actorData[actor] is NPCActorData, "Actor data is not of type NPCActorData.");
            (_actorData[actor] as NPCActorData).LastPetitionResource = type;
        }

        public override bool IsValidPetitionTarget(ActorTag2D actor, ResourceType type)
        {
            DebugTool.Assert(_actorData[actor] is NPCActorData, "Actor data is not of type NPCActorData.");
            return (_actorData[actor] as NPCActorData).IsValidPetitionTarget(type);
        }
    }
}