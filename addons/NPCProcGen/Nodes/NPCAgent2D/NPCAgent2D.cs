using System;
using System.Collections.Generic;
using Godot;
using NPCProcGen.Core.Actions;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Components.Variants;
using NPCProcGen.Core.Helpers;
using NPCProcGen.Core.Internal;
using NPCProcGen.Core.Traits;

namespace NPCProcGen
{
    /// <summary>
    /// Represents a 2D NPC agent with various traits and resource weights.
    /// Inherits from ActorTag2D.
    /// </summary>
    [Tool]
    public partial class NPCAgent2D : ActorTag2D
    {
        [Export(PropertyHint.Range, "1,100,")]
        public int SatiationValue { get; set; } = 100;

        /// <summary>
        /// Gets or sets the companionship value associated with this NPC.
        /// </summary>
        /// <value>The companionship value as an integer.</value>
        [Export(PropertyHint.Range, "1,100,")]
        public int CompanionshipValue { get; set; } = 100;

        /// <summary>
        /// Gets or sets the actor detector, which is an Area2D instance.
        /// When the actor detector is set to a new value, it updates the configuration warnings.
        /// </summary>
        /// <value>The Area2D instance representing the actor detector.</value>
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
        public float Satiation { get; set; } = 0.5f;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Companionship { get; set; } = 0.5f;

        // TODO: Derive event handler parameters from GodotObject

        [Signal]
        public delegate void ExecutionStartedEventHandler(Variant action);
        [Signal]
        public delegate void ExecutionEndedEventHandler(Variant action);

        [Signal]
        public delegate void ActionStateEnteredEventHandler(Variant state);
        [Signal]
        public delegate void ActionStateExitedEventHandler(Variant state);

        [Signal]
        public delegate void TheftCompletedEventHandler(TheftData theftData);

        /// <summary>
        /// Gets the target position for the NPC.
        /// </summary>
        public Vector2 TargetPosition => Executor.GetTargetPosition();

        /// <summary>
        /// Gets the sensor component of the NPC.
        /// </summary>
        public Sensor Sensor { get; private set; } = new();

        /// <summary>
        /// Gets the memorizer component of the NPC.
        /// </summary>
        public Memorizer Memorizer { get; private set; } = new();

        /// <summary>
        /// Gets the strategizer component of the NPC.
        /// </summary>
        public Strategizer Strategizer { get; private set; } = new();

        /// <summary>
        /// Gets the executor component of the NPC.
        /// </summary>
        public Executor Executor { get; private set; }

        /// <summary>
        /// Gets the notification manager of the NPC.
        /// </summary>
        public NotifManager NotifManager { get; private set; } = new();

        // TODO: Consider using raycast for detection
        private readonly List<ActorTag2D> _detectedActors = new();
        private Area2D _actorDetector = null;
        private Timer _evaluationTimer;

        /// <summary>
        /// Called when the node is added to the scene.
        /// Initializes the parent node and checks for required nodes.
        /// </summary>
        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            Parent = GetParent() as Node2D;

            if (Parent == null || _actorDetector == null || StealMarker == null)
            {
                QueueFree();
                return;
            }

            Executor = new(this);
            Executor.ExecutionEnded += OnExecutionEnded;

            _actorDetector.BodyEntered += OnBodyEntered;
            _actorDetector.BodyExited += OnBodyExited;

            _evaluationTimer = new()
            {
                WaitTime = 10,
                OneShot = true,
                Autostart = true
            };
            _evaluationTimer.Timeout += OnEvaluationTimerTimeout;
            AddChild(_evaluationTimer);

            AddTraits();
        }

        /// <summary>
        /// Called when the node enters the scene tree.
        /// Updates the parent node and configuration warnings if in the editor.
        /// </summary>
        public override void _EnterTree()
        {
            base._EnterTree();
        }

        /// <summary>
        /// Provides configuration warnings for the node.
        /// </summary>
        /// <returns>An array of warning messages.</returns>
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

        /// <summary>
        /// Called every frame. Updates the memorizer and executor components.
        /// </summary>
        /// <param name="delta">The frame time in seconds.</param>
        public override void _Process(double delta)
        {
            if (Engine.IsEditorHint()) return;

            Memorizer.Update(delta);
            Executor.Update(delta);
        }

        /// <summary>
        /// Initializes the NPC with a list of actors.
        /// </summary>
        /// <param name="actors">The list of actors to initialize with.</param>
        public void Initialize(List<ActorTag2D> actors)
        {
            Memorizer.Initialize(actors);
        }

        /// <summary>
        /// Checks if the specified actor is in range.
        /// </summary>
        /// <param name="actor">The actor to check.</param>
        /// <returns>True if the actor is in range, otherwise false.</returns>
        public bool IsActorInRange(ActorTag2D actor)
        {
            return _detectedActors.Contains(actor);
        }

        /// <summary>
        /// Checks if the NPC is currently active.
        /// </summary>
        /// <returns>True if the NPC is active, otherwise false.</returns>
        public bool IsActive()
        {
            return Executor.HasAction();
        }

        /// <summary>
        /// Checks if navigation is required for the NPC.
        /// </summary>
        /// <returns>True if navigation is required, otherwise false.</returns>
        public bool IsNavigationRequired()
        {
            return Executor.QueryNavigationAction();
        }

        /// <summary>
        /// Completes the navigation for the NPC.
        /// </summary>
        public void CompleteNavigation()
        {
            NotifManager.NotifyNavigationComplete();
        }

        /// <summary>
        /// Adds traits to the NPC's strategizer.
        /// </summary>
        private void AddTraits()
        {
            Strategizer.AddTrait(new SurvivalTrait(this, Survival, Sensor, Memorizer));

            if (Thief > 0)
                Strategizer.AddTrait(new ThiefTrait(this, Thief, Sensor, Memorizer));

            if (Lawful > 0)
                Strategizer.AddTrait(new LawfulTrait(this, Lawful, Sensor, Memorizer));
        }

        /// <summary>
        /// Handles the evaluation timer timeout event.
        /// Evaluates an action for the NPC.
        /// </summary>
        private void OnEvaluationTimerTimeout()
        {
            BaseAction action = Strategizer.EvaluateAction(SocialPractice.Proactive);

            if (action != null)
            {
                GD.Print($"Action evaluated by {Parent.Name}: {action.GetType().Name}");
                Executor.SetAction(action);
            }
            else
            {
                GD.Print($"No action evaluated by {Parent.Name}");
                _evaluationTimer.Start();
            }
        }

        /// <summary>
        /// Handles the execution ended event.
        /// Starts the evaluation timer.
        /// </summary>
        private void OnExecutionEnded()
        {
            _evaluationTimer.Start();
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
                    NotifManager.NotifyActorDetected(actor);
                }
            }
        }

        /// <summary>
        /// Handles the body exited event for the actor detector.
        /// Updates the memorizer with the actor's location and removes the actor from the detected list.
        /// </summary>
        /// <param name="body">The body that exited the actor detector.</param>
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
    }
}