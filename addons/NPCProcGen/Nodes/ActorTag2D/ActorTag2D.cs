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
    public partial class ActorTag2D : Node
    {
        [Signal]
        public delegate void InteractionStartedEventHandler(Variant state, Array<Variant> data);
        [Signal]
        public delegate void InteractionEndedEventHandler();

        [Signal]
        public delegate void EventTriggeredEventHandler(Variant eventType);

        [Export(PropertyHint.Range, "0,1000000,")]
        public int MoneyAmount { get; set; } = 100;

        [Export(PropertyHint.Range, "0,1000,")]
        public int FoodAmount { get; set; } = 5;

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
            Node2D parent = GetParent<Node2D>();

            Vector2 directionToInitiator = parent.GlobalPosition
                .DirectionTo(initiator.GetParent<Node2D>().GlobalPosition);
            return parent.GlobalPosition + directionToInitiator * CommonUtils.PositionOffset;
        }

        public void AnswerPetition(bool isAccepted)
        {
            NotifManager.Instance.NotifyPetitionAnswered(this, isAccepted);
        }

        public int GetFoodAmount()
        {
            return (int)ResourceManager.Instance.GetResource(this, ResourceType.Food).Amount;
        }

        public void AddFood(int amount)
        {
            ResourceManager.Instance.ModifyResource(this, ResourceType.Food, amount);
        }

        public void DeductFood(int amount)
        {
            ResourceManager.Instance.ModifyResource(this, ResourceType.Food, -amount);
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

        public virtual void TriggerInteraction(ActorTag2D target, InteractState state,
            Array<Variant> data)
        {
            NotifManager.Instance.NotifyInteractionStarted(this);
            Sensor.SetTaskRecord(ActionType.Interact, ActionState.Interact);

            CommonUtils.EmitSignal(
                this,
                SignalName.InteractionStarted,
                Variant.From(state),
                data
            );
        }

        public virtual void StopInteraction()
        {
            NotifManager.Instance.NotifyInteractionEnded(this);
            Sensor.ClearTaskRecord();
            CommonUtils.EmitSignal(this, SignalName.InteractionEnded);
        }

        public virtual void TriggerDetainment()
        {
            NotifManager.Instance.NotifyActorDetained(this);
            Sensor.SetAvailability(false);

            DetainNPC();

            CommonUtils.EmitSignal(
                this,
                SignalName.EventTriggered,
                Variant.From(EventType.Detained)
            );
        }

        public virtual void TriggerCaptivity()
        {
            NotifManager.Instance.NotifyActorCaptured(this);

            CommonUtils.EmitSignal(
                this,
                SignalName.EventTriggered,
                Variant.From(EventType.Captured)
            );
        }

        private void OnCrimeCommitted(ActorTag2D criminal, Crime crime)
        {
            if (criminal == this) return;

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
