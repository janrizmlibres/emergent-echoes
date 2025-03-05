using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;
using System.Collections.Generic;
using System.Linq;

namespace NPCProcGen
{
    [Tool]
    [GlobalClass]
    public partial class ActorTag2D : Node
    {
        [Signal]
        public delegate void InteractionStartedEventHandler(Variant state, Array<Variant> data);
        [Signal]
        public delegate void InteractionEndedEventHandler();

        [Signal]
        public delegate void EventTriggeredEventHandler(Variant eventType, Array<Variant> data);

        [Export(PropertyHint.Range, "0,1000000,")]
        public int MoneyAmount { get; set; } = 500;

        [Export(PropertyHint.Range, "0,1000,")]
        public int FoodAmount { get; set; } = 10;

        [Export]
        public Area2D ActorDetector { get; set; }
        [Export]
        public Marker2D RearMarker { get; set; }

        public Sensor Sensor { get; protected set; }

        public Memorizer Memorizer { get; protected set; }

        protected readonly List<ActorTag2D> _nearbyActors = new();

        public ActorTag2D()
        {
            NotifManager.Instance.CrimeCommitted += OnCrimeCommitted;
        }

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            ActorContext context = new(this);

            Sensor = new Sensor(context);
            Memorizer = new Memorizer(context);

            context.Sensor = Sensor;
            context.Memorizer = Memorizer;

            ActorDetector.BodyEntered += OnBodyEntered;
            ActorDetector.BodyExited += OnBodyExited;
        }

        public void Initialize(List<ActorTag2D> actors)
        {
            Memorizer.Initialize(actors);
        }

        public Vector2 GetRearPosition()
        {
            Node2D parent = GetParent<Node2D>();
            return parent.GlobalPosition + RearMarker.Position.Normalized()
                * CommonUtils.PositionOffset;
        }

        public Vector2 GetLateralWaypoint(ActorTag2D initiator)
        {
            Node2D parent = GetParent<Node2D>();
            Node2D initiatorParent = initiator.GetParent<Node2D>();

            float offset = CommonUtils.PositionOffset;

            Vector2 offset1 = new(offset, 0);
            Vector2 adjustedPosition1 = parent.GlobalPosition + offset1;
            float distance1 = initiatorParent.GlobalPosition.DistanceTo(adjustedPosition1);

            Vector2 offset2 = new(-offset, 0);
            Vector2 adjustedPosition2 = parent.GlobalPosition + offset2;
            float distance2 = initiatorParent.GlobalPosition.DistanceTo(adjustedPosition2);

            // Return the target's position adjusted by the best offset
            return distance1 < distance2 ? adjustedPosition1 : adjustedPosition2;
        }

        public Vector2 GetOmniDirectionalWaypoint(ActorTag2D initiator)
        {
            Vector2 origin = initiator.GetParent<Node2D>().GlobalPosition;
            Vector2 target = GetParent<Node2D>().GlobalPosition;
            return CommonUtils.GetOmnidirectionalWaypoint(origin, target);
        }

        public void AnswerPetition(bool isAccepted)
        {
            NotifManager.Instance.NotifyPetitionAnswered(this, isAccepted);
        }

        public int GetFoodAmount()
        {
            return (int)ResourceManager.Instance.GetResource(ResourceType.Food, this).Amount;
        }

        public void AddFood(int amount)
        {
            ResourceManager.Instance.ModifyResource(ResourceType.Food, amount, this);
        }

        public void DeductFood(int amount)
        {
            ResourceManager.Instance.ModifyResource(ResourceType.Food, -amount, this);
        }

        public bool IsPlayer()
        {
            return GetType() != typeof(NPCAgent2D);
        }

        public bool IsValidTarget(NPCAgent2D agent)
        {
            Vector2? actorLastPos = agent.Memorizer.GetLastKnownPosition(this);

            if (actorLastPos == null && !agent.IsActorInRange(this)) return false;
            if (IsPlayer() && GD.Randf() > 0.2) return false;
            if (Sensor.IsUnavailable()) return false;
            return true;
        }

        public bool CanInteract()
        {
            if (Sensor.IsBusy()) return false;
            if (Sensor.IsUnavailable()) return false;
            return true;
        }

        public void TriggerInteraction(ActorTag2D target, InteractionState state,
            Array<Variant> data)
        {
            ExecuteTriggerInteraction(target);

            CommonUtils.EmitSignal(
                this,
                SignalName.InteractionStarted,
                Variant.From(state),
                data
            );
        }

        public void StopInteraction()
        {
            ExecuteStopInteraction();
            CommonUtils.EmitSignal(this, SignalName.InteractionEnded);
        }

        protected virtual void ExecuteTriggerInteraction(ActorTag2D target)
        {
            NotifManager.Instance.NotifyInteractionStarted(this);
            Sensor.SetTaskRecord(ActionType.Interact, ActionState.Interact);
        }

        protected virtual void ExecuteStopInteraction()
        {
            NotifManager.Instance.NotifyInteractionEnded(this);
            Sensor.ClearTaskRecord();
        }

        public virtual void TriggerDetainment(ActorTag2D captor)
        {
            NotifManager.Instance.NotifyActorDetained(this, captor);
            Sensor.SetAvailability(false);

            DetainNPC();

            CommonUtils.EmitSignal(
                this,
                SignalName.EventTriggered,
                Variant.From(EventType.Detained),
                new Array<Variant>()
            );
        }

        public virtual void TriggerCaptivity(Vector2 releaseLocation)
        {
            NotifManager.Instance.NotifyActorCaptured(this);

            CommonUtils.EmitSignal(
                this,
                SignalName.EventTriggered,
                Variant.From(EventType.Captured),
                new Array<Variant> { releaseLocation }
            );
        }

        private void OnCrimeCommitted(ActorTag2D criminal, Crime crime)
        {
            if (criminal == this) return;

            ExecuteOnCrimeCommitted(criminal, crime);

            if (_nearbyActors.Contains(criminal))
            {
                crime.Participants.Add(this);

                CommonUtils.EmitSignal(
                    this,
                    SignalName.EventTriggered,
                    Variant.From(EventType.CrimeWitnessed)
                );
            }
        }

        protected virtual void ExecuteOnCrimeCommitted(ActorTag2D criminal, Crime crime) { }

        protected void OnBodyEntered(Node2D body)
        {
            if (GetParent<Node2D>() == body) return;

            ActorTag2D actor = body.GetChildren().OfType<ActorTag2D>().FirstOrDefault();

            if (actor != null)
            {
                _nearbyActors.Add(actor);
                OnNPCActorEntered(actor);
            }
        }

        protected void OnBodyExited(Node2D body)
        {
            if (GetParent<Node2D>() == body) return;

            ActorTag2D actor = body.GetChildren().OfType<ActorTag2D>().FirstOrDefault();

            if (actor != null)
            {
                Vector2 rememberedPosition = actor.GetParent<Node2D>().GlobalPosition;
                Memorizer.UpdateLastKnownPosition(actor, rememberedPosition);
                _nearbyActors.Remove(actor);
            }
        }

        protected virtual void DetainNPC() { }
        protected virtual void OnNPCActorEntered(ActorTag2D actor) { }
    }
}
