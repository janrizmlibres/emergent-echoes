using System.Collections.Generic;
using Godot;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.Traits;

namespace NPCProcGen
{
    [Tool]
    public partial class NPCAgent2D : ActorTag2D
    {
        [Export(PropertyHint.Range, "1,100,")]
        public int CompanionshipValue { get; set; } = 100;

        [Export]
        public Area2D ActorDetector
        {
            get => _actorDetector;
            set
            {
                if (value != _actorDetector)
                {
                    _actorDetector = value;
                    UpdateConfigurationWarnings();
                }
            }
        }

        [ExportGroup("Traits")]

        [Export(PropertyHint.Range, "0.01,1,0.01")]
        public float Survival { get; set; } = 0.1f;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Thief { get; set; } = 0;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Lawful { get; set; } = 0;
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
        public float Companionship { get; set; } = 0.5f;

        [Signal]
        public delegate void ExecutionStartedEventHandler();
        [Signal]
        public delegate void ExecutionEndedEventHandler();

        public Vector2 TargetPosition => Executor.GetTargetPosition();

        public Sensor Sensor { get; private set; } = new();
        public Memorizer Memorizer { get; private set; } = new();
        public Strategizer Strategizer { get; private set; } = new();
        public Executor Executor { get; private set; } = new();

        // TODO: Consider using raycast for detection
        private readonly List<ActorTag2D> _detectedActors = new();

        private readonly Timer _evaluationTimer = new();
        private Area2D _actorDetector = null;

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            Parent = GetParent() as Node2D;

            if (_actorDetector == null || Parent == null || StealMarker == null)
            {
                QueueFree();
                return;
            }

            _evaluationTimer.WaitTime = 10;
            _evaluationTimer.OneShot = true;
            _evaluationTimer.Timeout += OnEvaluationTimerTimeout;

            AddChild(_evaluationTimer);
            _evaluationTimer.Start();

            _actorDetector.BodyEntered += OnActorEntered;
            _actorDetector.BodyExited += OnActorExited;
            Executor.OnExecutionEnded += OnExecutionEnded;

            AddTraits();
            AddResources();
        }

        public override void _EnterTree()
        {
            base._EnterTree();
        }

        public override string[] _GetConfigurationWarnings()
        {
            List<string> warnings = new();

            if (Parent == null)
            {
                warnings.Add("The NPCAgent2D can be used only under a Node2D inheriting parent node.");
            }

            if (_actorDetector == null)
            {
                warnings.Add("The NPCAgent2D requires an Area2D to detect other actors.");
            }

            if (StealMarker == null)
            {
                warnings.Add("The NPCAgent2D requires a Marker2D node to make stealing possible.");
            }

            return warnings.ToArray();
        }

        public override void _Process(double delta)
        {
            if (Engine.IsEditorHint()) return;

            Memorizer.ProcessUpdates(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Engine.IsEditorHint()) return;

            Executor.Update(delta);
        }

        public void CompleteNavigation()
        {
            Executor.NotifyNavigationState();
        }

        public void CompleteTheft()
        {
            Executor.NotifyStateChange();
        }

        public void Initialize(List<ActorTag2D> actors)
        {
            Memorizer.Initialize(actors);

            // ! Remove debug print
            // GD.Print($"Actor memory of {Parent.Name}:");
            // _memorizer.PrintActorMemory();
        }

        public bool IsActorInRange(ActorTag2D actor)
        {
            return _detectedActors.Contains(actor);
        }

        public bool IsActive()
        {
            return Executor.HasAction();
        }

        public bool IsNavigationRequired()
        {
            return Executor.QueryNavigationState();
        }

        public bool IsStealing()
        {
            return Executor.QueryStealState();
        }

        private void AddTraits()
        {
            Strategizer.AddTrait(new SurvivalTrait(this, Survival, Sensor, Memorizer));

            if (Thief > 0)
                Strategizer.AddTrait(new ThiefTrait(this, Thief, Sensor, Memorizer));

            if (Lawful > 0)
                Strategizer.AddTrait(new LawfulTrait(this, Lawful, Sensor, Memorizer));
        }

        private void AddResources()
        {
            ResourceStat money = new(ResourceType.Money, MoneyValue, Money);
            ResourceStat food = new(ResourceType.Food, FoodValue, Food);
            ResourceStat companionship = new(ResourceType.Companionship, CompanionshipValue, Companionship);

            Resources.Add(ResourceType.Money, money);
            Resources.Add(ResourceType.Food, food);
            Resources.Add(ResourceType.Companionship, companionship);
        }

        private void OnEvaluationTimerTimeout()
        {
            NPCAction action = Strategizer.EvaluateAction(SocialPractice.Proactive);

            if (action != null)
            {
                // ! Remove debug print in production
                GD.Print($"{Parent.Name} performs action: " + action.GetType().Name);
                Executor.SetAction(action);
                EmitSignal(SignalName.ExecutionStarted);
            }
        }

        private void OnActorEntered(Node2D body)
        {
            foreach (Node child in body.GetChildren())
            {
                if (child is ActorTag2D actor)
                {
                    _detectedActors.Add(actor);
                }
            }
        }

        private void OnActorExited(Node2D body)
        {
            foreach (Node child in body.GetChildren())
            {
                if (child is ActorTag2D actor)
                {
                    Memorizer.UpdateActorLocation(actor, actor.Parent.GlobalPosition);
                    _detectedActors.Remove(actor);
                }
            }
        }

        private void OnExecutionEnded()
        {
            EmitSignal(SignalName.ExecutionEnded);
            _evaluationTimer.Start();
        }
    }
}