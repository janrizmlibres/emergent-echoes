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

        private const float EVALUATION_INTERVAL = 30;

        private readonly Sensor _sensor = new();
        private readonly Memorizer _memorizer = new();
        private readonly Strategizer _strategizer = new();
        private readonly Executor _executor = new();

        private float _evaluationTimer = 0;

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            _parent = GetParent() as Node2D;

            if (_parent == null)
            {
                SetProcess(false);
                SetPhysicsProcess(false);
            }

            AddTraits();
            AddResources();
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

            _memorizer.UpdateActorData(delta);
            _executor.Update();

            if (!_executor.IsExecuting())
            {
                WaitForEvaluation(delta);
            }
        }

        private void WaitForEvaluation(double delta)
        {
            _evaluationTimer += (float)delta;

            if (_evaluationTimer >= EVALUATION_INTERVAL)
            {
                NPCAction action = _strategizer.EvaluateAction(SocialPractice.Proactive);
                _executor.SetAction(action);
                _evaluationTimer = 0;
            }
        }
    }
}