using System;
using System.Collections.Generic;
using Godot;
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

        private static readonly float _evaluationInterval = 10;

        public Vector2 TargetPosition => Executor.GetTargetPosition();

        public Sensor Sensor { get; private set; } = new();
        public Memorizer Memorizer { get; private set; } = new();
        public Strategizer Strategizer { get; private set; } = new();
        public Executor Executor { get; private set; } = new();

        public NotifManager NotifManager { get; private set; } = new();

        // TODO: Consider using raycast for detection
        private readonly List<ActorTag2D> _detectedActors = new();
        private Area2D _actorDetector = null;

        private float _evaluationTimer = 0;

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            Parent = GetParent() as Node2D;

            if (Parent == null || _actorDetector == null || StealMarker == null)
            {
                QueueFree();
                return;
            }

            _actorDetector.BodyEntered += OnBodyEntered;
            _actorDetector.BodyExited += OnBodyExited;

            Executor.ExecutionEnded += OnExecutionEnded;

            AddTraits();
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

            if (!Executor.HasAction())
            {
                _evaluationTimer += (float)delta;

                if (_evaluationTimer >= _evaluationInterval)
                {
                    _evaluationTimer = 0;
                    OnEvaluationTimerTimeout();
                }
            }

            Memorizer.Update(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Engine.IsEditorHint()) return;

            Executor.Update(delta);
        }

        public void Initialize(List<ActorTag2D> actors)
        {
            Memorizer.Initialize(actors);
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
            return Executor.QueryNavigationAction();
        }

        public bool IsStealing()
        {
            return Executor.QueryStealState();
        }

        public void CompleteNavigation()
        {
            NotifManager.NotifyNavigationComplete();
        }

        public void CompleteTheft()
        {
            NotifManager.NotifyTheftComplete();
        }

        public Tuple<ResourceType, float> GetStolenResource()
        {
            return Executor.QueryStolenResource();
        }

        private void AddTraits()
        {
            Strategizer.AddTrait(new SurvivalTrait(this, Survival, Sensor, Memorizer));

            if (Thief > 0)
                Strategizer.AddTrait(new ThiefTrait(this, Thief, Sensor, Memorizer));

            if (Lawful > 0)
                Strategizer.AddTrait(new LawfulTrait(this, Lawful, Sensor, Memorizer));
        }

        private void OnEvaluationTimerTimeout()
        {
            NPCAction action = Strategizer.EvaluateAction(SocialPractice.Proactive);

            if (action != null)
            {
                GD.Print("Action evaluated: " + action.GetType().Name);
                Executor.SetAction(action);
                EmitSignal(SignalName.ExecutionStarted);
            }
        }

        // TODO: Resolve self detection in editor
        private void OnBodyEntered(Node2D body)
        {
            if (Parent == body) return;

            foreach (Node child in body.GetChildren())
            {
                if (child is ActorTag2D actor)
                {
                    _detectedActors.Add(actor);
                }
            }
        }

        private void OnBodyExited(Node2D body)
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
        }
    }
}