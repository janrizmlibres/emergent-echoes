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

        public Vector2 TargetPosition
        {
            get => _executor.GetTargetPosition();
        }

        private readonly Timer _evaluationTimer = new();
        private Area2D _actorDetector = null;

        private readonly Sensor _sensor = new();
        private readonly Memorizer _memorizer = new();
        private readonly Strategizer _strategizer = new();
        private readonly Executor _executor = new();

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

            _actorDetector.BodyEntered += OnActorDetected;
            _executor.OnExecutionEnded += OnExecutionEnded;

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

            _memorizer.ProcessUpdates(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Engine.IsEditorHint()) return;

            _executor.Update(delta);
        }

        public void CompleteNavigation()
        {
            _executor.NotifyNavigationState();
        }

        public void CompleteTheft()
        {
            _executor.NotifyStealState();
        }

        public void Initialize(List<ActorTag2D> actors)
        {
            _memorizer.Initialize(actors);

            // ! Remove debug print
            // GD.Print($"Actor memory of {Parent.Name}:");
            // _memorizer.PrintActorMemory();
        }

        public bool IsActive()
        {
            return _executor.HasAction();
        }

        public bool IsNavigationRequired()
        {
            return _executor.IsNavigationRequired();
        }

        public bool CanSteal()
        {
            return _executor.CanSteal();
        }

        private void AddTraits()
        {
            _strategizer.AddTrait(new SurvivalTrait(this, Survival, _sensor, _memorizer));

            if (Thief > 0)
                _strategizer.AddTrait(new ThiefTrait(this, Thief, _sensor, _memorizer));

            if (Lawful > 0)
                _strategizer.AddTrait(new LawfulTrait(this, Lawful, _sensor, _memorizer));
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
            NPCAction action = _strategizer.EvaluateAction(SocialPractice.Proactive);

            if (action != null)
            {
                // ! Remove debug print in production
                GD.Print($"{Parent.Name} performs action: " + action.GetType().Name);
                _executor.SetAction(action);
                EmitSignal(SignalName.ExecutionStarted);
            }
        }

        private void OnActorDetected(Node2D body)
        {
            foreach (Node child in body.GetChildren())
            {
                if (child is ActorTag2D actor && child != this)
                {
                    _memorizer.UpdateActorLocation(actor, actor.Parent.GlobalPosition);
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