using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Represents a 2D NPC agent with various traits and resource weights.
    /// Inherits from ActorTag2D.
    /// </summary>
    [Tool]
    public partial class NPCAgent2D : ActorTag2D
    {
        [Signal]
        public delegate void ExecutionStartedEventHandler(Variant action);
        [Signal]
        public delegate void ExecutionEndedEventHandler();

        [Signal]
        public delegate void ActionStateEnteredEventHandler(Variant state, Array<Variant> data);
        [Signal]
        public delegate void ActionStateExitedEventHandler(Variant state, Array<Variant> data);

        private const int MinEvaluationInterval = 10;
        private const int MaxEvaluationInterval = 20;

        [Export(PropertyHint.Range, "1,100,")]
        public int SatiationAmount { get; set; } = 15;

        /// <summary>
        /// Gets or sets the companionship value associated with this NPC.
        /// </summary>
        /// <value>The companionship value as an integer.</value>
        [Export(PropertyHint.Range, "1,100,")]
        public int CompanionshipAmount { get; set; } = 10;

        [Export(PropertyHint.Range, "1,500,")]
        public int ActorDetectionRange { get; set; } = 70;

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
        public float Satiation { get; set; } = 0.5f;
        [Export(PropertyHint.Range, "0,1,0.01")]
        public float Companionship { get; set; } = 0.5f;

        /// <summary>
        /// Gets the target position for the NPC.
        /// </summary>
        public Vector2 TargetPosition => Executor.GetTargetPosition();

        /// <summary>
        /// Gets the strategizer component of the NPC.
        /// </summary>
        public Strategizer Strategizer { get; private set; } = new();

        /// <summary>
        /// Gets the executor component of the NPC.
        /// </summary>
        public Executor Executor { get; private set; }

        public List<Trait> Traits { get; private set; } = new();

        // TODO: Consider using raycast for detection
        private readonly List<ActorTag2D> _detectedActors = new();

        private Area2D _actorDetector;
        private Timer _evaluationTimer;

        /// <summary>
        /// Called when the node enters the scene tree.
        /// Updates the parent node and configuration warnings if in the editor.
        /// </summary>
        public override void _EnterTree()
        {
            base._EnterTree();
        }

        /// <summary>
        /// Called when the node is added to the scene.
        /// Initializes the parent node and checks for required nodes.
        /// </summary>
        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;

            Parent = GetParent() as Node2D;
            Sensor = new Sensor(this);
            Memorizer = new NPCMemorizer(this);

            if (Parent == null || RearMarker == null)
            {
                QueueFree();
                return;
            }

            // NotifManager.InteractionStarted += OnInteractionStarted;
            // NotifManager.InteractionEnded += OnInteractionEnded;
            Executor = new(this);
            Executor.ExecutionEnded += OnExecutionEnded;

            _actorDetector = new Area2D();
            CollisionShape2D collisionShape2D = new();
            CircleShape2D circleShape2D = new()
            {
                Radius = ActorDetectionRange
            };

            collisionShape2D.Shape = circleShape2D;

            _actorDetector.AddChild(collisionShape2D);
            CallDeferred(MethodName.SetActorDetector, _actorDetector);

            _actorDetector.BodyEntered += OnBodyEntered;
            _actorDetector.BodyExited += OnBodyExited;

            _evaluationTimer = new Timer()
            {
                WaitTime = GD.RandRange(MinEvaluationInterval, MaxEvaluationInterval),
                OneShot = true,
                Autostart = true
            };
            _evaluationTimer.Timeout += OnEvaluationTimerTimeout;
            AddChild(_evaluationTimer);
            AddTraits();
            // AddTraitsStub();
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

            if (RearMarker == null)
            {
                warnings.Add(@"The NPCAgent2D requires a Marker2D node to identify the rear side 
                    of the actor.");
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

            ResourceManager.Instance.Update(delta);

            Traits.ForEach(trait => trait.Update(delta));
            Memorizer.Update(delta);
            Executor.Update(delta);
        }

        public override void Arrest()
        {
            NotifManager.NotifyActorImprisoned();
            Executor.EndAllActions();
            _evaluationTimer.Stop();
            _isImprisoned = true;
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

        public bool IsAnyActorInRange()
        {
            return _detectedActors.Any();
        }

        public List<ActorTag2D> GetActorsInRange()
        {
            return _detectedActors.ToList();
        }

        public ActorTag2D GetRandomActorInRange()
        {
            return CommonUtils.Shuffle(_detectedActors).FirstOrDefault();
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
            return Executor.IsNavigationRequired();
        }

        public Tuple<ActionType, ActionState> GetAction()
        {
            return Sensor.GetTaskRecord();
        }

        /// <summary>
        /// Completes the navigation for the NPC.
        /// </summary>
        public bool CompleteNavigation()
        {
            return Executor.CompleteNavigation();
        }

        public void CompleteConsumption()
        {
            Executor.CompleteConsumption();
        }

        /// <summary>
        /// Adds traits to the NPC's strategizer.
        /// </summary>
        private void AddTraits()
        {
            DebugTool.Assert(Memorizer is NPCMemorizer, "Memorizer is not of type NPCMemorizer");
            NPCMemorizer npcMemorizer = Memorizer as NPCMemorizer;

            Traits.Add(new SurvivalTrait(this, Survival, Sensor, npcMemorizer));

            if (Thief > 0)
                Traits.Add(new ThiefTrait(this, Thief, Sensor, npcMemorizer));

            if (Lawful > 0)
                Traits.Add(new LawfulTrait(this, Lawful, Sensor, npcMemorizer));

            Strategizer.InitializeTraits(Traits);
        }

        // ! Remove in production
        private void AddTraitsStub()
        {
            DebugTool.Assert(Memorizer is NPCMemorizer, "Memorizer is not of type NPCMemorizer");
            NPCMemorizer npcMemorizer = Memorizer as NPCMemorizer;

            Traits.Add(new SurvivalTrait(this, Survival, Sensor, npcMemorizer));
            Traits.Add(new ThiefTrait(this, Thief, Sensor, npcMemorizer));
            Traits.Add(new LawfulTrait(this, Lawful, Sensor, npcMemorizer));
            Strategizer.InitializeTraits(Traits);
        }

        private void SetActorDetector(Area2D actorDetector)
        {
            Parent.AddChild(actorDetector);
        }

        private void OnEvaluationTimerTimeout()
        {
            // BaseAction action = Strategizer.EvaluateActionStub(
            //     typeof(ThiefTrait),
            //     typeof(TheftAction),
            //     ResourceType.Money
            // );

            BaseAction action = Strategizer.EvaluateAction(SocialPractice.Proactive);

            if (action != null)
            {
                GD.Print($"Action evaluated by {Parent.Name}: {action.GetType().Name}");
                Executor.AddAction(action);
            }
            else
            {
                _evaluationTimer.Start(GD.RandRange(MinEvaluationInterval, MaxEvaluationInterval));
            }
        }

        /// <summary>
        /// Handles the execution ended event.
        /// Starts the evaluation timer.
        /// </summary>
        private void OnExecutionEnded()
        {
            _evaluationTimer.Start(GD.RandRange(MinEvaluationInterval, MaxEvaluationInterval));
            Error result = EmitSignal(SignalName.ExecutionEnded);
            DebugTool.Assert(result != Error.Unavailable, "Signal emitted error");
        }

        public void AddAction(BaseAction action)
        {
            Executor.AddAction(action);
            _evaluationTimer.Stop();
        }

        public void EndAction()
        {
            Executor.EndCurrentAction();
        }

        // TODO: Resolve self detection in editor
        private void OnBodyEntered(Node2D body)
        {
            if (Parent == body) return;

            ActorTag2D actor = body.GetChildren().OfType<ActorTag2D>().FirstOrDefault();

            if (actor != null)
            {
                _detectedActors.Add(actor);
                Executor.OnActorDetected(actor);
            }
        }

        /// <summary>
        /// Handles the body exited event for the actor detector.
        /// Updates the memorizer with the actor's location and removes the actor from the detected list.
        /// </summary>
        /// <param name="body">The body that exited the actor detector.</param>
        private void OnBodyExited(Node2D body)
        {
            ActorTag2D actor = body.GetChildren().OfType<ActorTag2D>().FirstOrDefault();

            if (actor != null)
            {
                Memorizer.UpdateLastKnownPosition(actor, actor.Parent.GlobalPosition);
                _detectedActors.Remove(actor);
            }
        }
    }
}