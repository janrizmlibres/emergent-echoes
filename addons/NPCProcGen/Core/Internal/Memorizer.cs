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
        protected readonly NPCAgent2D _owner;

        protected readonly Dictionary<ActorTag2D, ActorData> _actorData = new();

        public Memorizer(NPCAgent2D owner)
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
            UpdateActorData(delta);
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

        protected void UpdateActorData(double delta)
        {
            foreach (ActorData actorData in _actorData.Values)
            {
                actorData.Update(delta);
            }
        }

        public virtual Crime StartInvestigation() => null;
        public virtual Crime GetInvestigation() => null;
        public virtual void ClearInvestigation() { }
        public virtual bool IsInvestigating() => false;

        public virtual Vector2? GetLastKnownPosition(ActorTag2D actor) => null;
        public virtual void UpdateLastKnownPosition(ActorTag2D actor, Vector2 location) { }
        public virtual void UpdateLastPetitionResource(ActorTag2D actor, ResourceType type) { }
        public virtual bool IsValidPetitionTarget(ActorTag2D actor, ResourceType type) => false;
    }

    /// <summary>
    /// The Memorizer class is responsible for managing long-term and short-term memories of actors.
    /// </summary>
    public class NPCMemorizer : Memorizer
    {
        private const float CrimeInvestigationTime = 600;

        private Crime _currentInvestigation = null;
        private float _investigationTimer = CrimeInvestigationTime;

        public NPCMemorizer(NPCAgent2D owner) : base(owner) { }

        /// <summary>
        /// Initializes the memorizer with a list of actors.
        /// </summary>
        /// <param name="actors">The list of actors to initialize.</param>
        public override void Initialize(List<ActorTag2D> actors)
        {
            foreach (ActorTag2D actor in actors)
            {
                _actorData.Add(actor, new NPCActorData(actor));
            }
        }

        public override void Update(double delta)
        {
            UpdateActorData(delta);

            if (_currentInvestigation == null) return;

            _investigationTimer -= (float)delta;

            if (_investigationTimer <= 0)
            {
                _currentInvestigation = null;
                _owner.Sensor.CloseInvestigation();
            }
        }

        public override Crime StartInvestigation()
        {
            _currentInvestigation = _owner.Sensor.InvestigateCrime();
            if (_currentInvestigation == null) return null;

            _investigationTimer = CrimeInvestigationTime;
            return _currentInvestigation;
        }

        public override Crime GetInvestigation() => _currentInvestigation;

        public override void ClearInvestigation()
        {
            _currentInvestigation = null;
        }

        public override bool IsInvestigating() => _currentInvestigation != null;

        /// <summary>
        /// Gets the last known location of the specified actor.
        /// </summary>
        /// <param name="actor">The actor to get the location for.</param>
        /// <returns>The last known location of the actor, or null if not found.</returns>
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