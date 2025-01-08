using System;
using System.Collections.Generic;
using System.Linq;
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
        public delegate void MoveToTargetEventHandler(Vector2 target);

        public event Action OnFinishNavigation;

        private readonly Sensor _sensor = new();
        private readonly Memorizer _memorizer = new();
        private readonly Strategizer _strategizer = new();
        private readonly Executor _executor = new();

        private readonly Timer _evaluationTimer = new();
        private Area2D _actorDetector;

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
                warnings.Add("The NPCAgent2D requires a Marker2D node for use in actions such as stealing.");
            }

            return warnings.ToArray();
        }

        public override void _Process(double delta)
        {
            if (Engine.IsEditorHint()) return;

            _memorizer.ProcessActorUpdates(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Engine.IsEditorHint()) return;

            _executor.Update();
        }

        public void ReturnToIdle()
        {
            _executor.Action = null;
            _evaluationTimer.Start();
        }

        public void Initialize(List<ActorTag2D> actors)
        {
            _memorizer.Initialize(actors);
        }

        public bool ShouldMove()
        {
            return _executor.Action != null;
        }

        public void FinishNavigation()
        {
            OnFinishNavigation?.Invoke();
        }

        private void OnEvaluationTimerTimeout()
        {
            GD.Print("Evaluating action");
            NPCAction action = _strategizer.EvaluateAction(SocialPractice.Proactive);
            _executor.Action = action;
        }

        private void OnActorDetected(Node2D body)
        {
            foreach (Node child in body.GetChildren())
            {
                if (child is ActorTag2D)
                {
                    ActorTag2D actor = child as ActorTag2D;
                    _memorizer.UpdateActorLocation(actor, actor.GetParentGlobalPosition());
                }
            }
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
    }
}