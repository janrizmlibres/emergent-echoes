using System.Collections.Generic;
using System.Linq;
using EmergentEchoes.Entities.Hooks;
using Godot;
using Godot.Collections;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.Traits;

namespace NPCProcGen
{
    [Tool]
    public partial class NPCAgent2D : ActorTag2D
    {
        [Signal]
        public delegate void ExecutionStartedEventHandler();
        [Signal]
        public delegate void ExecutionEndedEventHandler();

        [Signal]
        public delegate void ActionStartedEventHandler(Variant action);
        [Signal]
        public delegate void ActionEndedEventHandler();

        [Signal]
        public delegate void StateEnteredEventHandler(Variant state, Array<Variant> data);
        [Signal]
        public delegate void StateExitedEventHandler(Variant state, Array<Variant> data);

        private const int MinWaitTime = 10;
        private const int MaxWaitTime = 20;

        [Export(PropertyHint.Range, "1,100,")]
        public int SatiationAmount { get; set; } = 15;

        [Export(PropertyHint.Range, "1,100,")]
        public int CompanionshipAmount { get; set; } = 10;

        [Export]
        public Area2D ActorDetector { get; set; }
        [Export]
        public Timer EvaluationTimer { get; set; }

        [ExportGroup("Traits")]

        [Export(PropertyHint.Range, "0.01,1,0.01")]
        public float Survival { get; set; } = 0.1f;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Thief { get; set; } = 0;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Lawful { get; set; } = 0;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Farmer { get; set; } = 0;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Generous { get; set; } = 0;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Violent { get; set; } = 0;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Social { get; set; } = 0;

        [ExportGroup("Resource Weights")]

        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Money { get; set; } = 0.5f;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Food { get; set; } = 0.5f;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Satiation { get; set; } = 0.5f;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Companionship { get; set; } = 0.5f;

        public List<Trait> Traits { get; private set; } = new();
        public Strategizer Strategizer { get; private set; }
        public Executor Executor { get; private set; }

        public Vector2 TargetPosition => Executor.GetTargetPosition();

        private readonly List<ActorTag2D> _nearbyActors = new();

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            ActorContext context = new(this);

            Sensor = new Sensor(context);
            Memorizer = new NPCMemorizer(context);
            Strategizer = new Strategizer(context);
            Executor = new Executor(context);

            context.Sensor = Sensor;
            context.Memorizer = Memorizer;
            context.Executor = Executor;

            ActorDetector.BodyEntered += OnBodyEntered;
            ActorDetector.BodyExited += OnBodyExited;

            EvaluationTimer.Timeout += OnEvaluationTimerTimeout;

            // AddTraits(context);
            AddTraitsStub(context);
        }

        public override void _Process(double delta)
        {
            if (Engine.IsEditorHint()) return;

            ResourceManager.Instance.Update(delta);

            Traits.ForEach(trait => trait.Update(delta));
            Memorizer.Update(delta);
            Executor.Update(delta);
        }

        public bool IsActorInRange(ActorTag2D actor)
        {
            return _nearbyActors.Contains(actor);
        }

        public bool IsAnyActorInRange()
        {
            return _nearbyActors.Any();
        }

        public List<ActorTag2D> GetActorsInRange()
        {
            return _nearbyActors.ToList();
        }

        public ActorTag2D GetRandomActorInRange()
        {
            return CommonUtils.Shuffle(_nearbyActors).FirstOrDefault();
        }

        public override void TriggerInteraction(ActorTag2D target, ActionState _actionState,
            Array<Variant> data)
        {
            ActorContext context = new(this)
            {
                Sensor = Sensor,
                Memorizer = Memorizer,
                Executor = Executor
            };

            InteractAction action = new(context, target);
            Executor.AddAction(action);
            EvaluationTimer.Stop();
        }

        public override void StopInteraction()
        {
            Executor.FinishAction();
        }

        public bool IsActive()
        {
            return Executor.HasAction();
        }

        public bool IsNavigationRequired()
        {
            return Executor.IsNavigationRequired();
        }

        public bool CompleteNavigation()
        {
            return Executor.CompleteNavigation();
        }

        public void CompleteConsumption()
        {
            Executor.CompleteConsumption();
        }

        private void AddTraits(ActorContext context)
        {
            Traits.Add(new SurvivalTrait(context, Survival));

            if (Thief > 0)
                Traits.Add(new ThiefTrait(context, Thief));

            if (Lawful > 0)
                Traits.Add(new LawfulTrait(context, Lawful));
        }

        private void OnEvaluationTimerTimeout()
        {
            BaseAction action = Strategizer.EvaluateActionStub(
                typeof(SurvivalTrait),
                typeof(PetitionAction),
                ResourceType.Money
            );

            // BaseAction action = Strategizer.EvaluateAction(SocialPractice.Proactive);

            if (action != null)
            {
                GD.Print($"Action evaluated by {GetParent<Node2D>().Name}: {action.GetType().Name}");
                Executor.AddAction(action);
            }
            else
            {
                EvaluationTimer.Start(GD.RandRange(MinWaitTime, MaxWaitTime));
            }
        }

        public void OnExecutionEnded()
        {
            EvaluationTimer.Start(GD.RandRange(MinWaitTime, MaxWaitTime));
        }

        private void OnBodyEntered(Node2D body)
        {
            if (GetParent<Node2D>() == body) return;

            ActorTag2D actor = body.GetChildren().OfType<ActorTag2D>().FirstOrDefault();

            if (actor != null)
            {
                _nearbyActors.Add(actor);
                Executor.OnActorDetected(actor);
            }
        }

        private void OnBodyExited(Node2D body)
        {
            ActorTag2D actor = body.GetChildren().OfType<ActorTag2D>().FirstOrDefault();

            if (actor != null)
            {
                Memorizer.UpdateLastKnownPosition(actor, actor.GetParent<Node2D>().GlobalPosition);
                _nearbyActors.Remove(actor);
            }
        }

        // ! Remove in production
        private void AddTraitsStub(ActorContext context)
        {
            Traits.Add(new SurvivalTrait(context, Survival));
            Traits.Add(new ThiefTrait(context, Thief));
            Traits.Add(new LawfulTrait(context, Lawful));
        }
    }
}