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

        private readonly Sensor _sensor = new();
        private readonly Memorizer _memorizer = new();
        private readonly Strategizer _strategizer = new();
        private readonly Executor _executor = new();

        private readonly Timer _evaluationTimer = new();
        private Area2D _actorDetector;

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            if (_actorDetector == null || _parent == null)
            {
                QueueFree();
                return;
            }

            _evaluationTimer.WaitTime = 30;
            _evaluationTimer.OneShot = true;
            _evaluationTimer.Timeout += OnEvaluationTimerTimeout;
            _evaluationTimer.Start();

            AddTraits();
            AddResources();
        }

        public override void _EnterTree()
        {
            if (Engine.IsEditorHint())
            {
                CheckParent();
            }
        }

        public override string[] _GetConfigurationWarnings()
        {
            List<string> warnings = new();

            if (_parent == null)
            {
                warnings.Add("The NPCAgent2D can be used only under a Node2D inheriting parent node.");
            }

            if (_actorDetector == null)
            {
                warnings.Add("The NPCAgent2D requires an Area2D to detect other actors.");
            }

            return warnings.ToArray();
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

        public override void _Process(double delta)
        {
            if (Engine.IsEditorHint()) return;

            _memorizer.UpdateActorData(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Engine.IsEditorHint()) return;

            _executor.Update();
        }

        private void OnEvaluationTimerTimeout()
        {
            NPCAction action = _strategizer.EvaluateAction(SocialPractice.Proactive);
            _executor.SetAction(action);
        }
    }
}