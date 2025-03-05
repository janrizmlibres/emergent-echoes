using System.Linq;
using EmergentEchoes.Entities.Hooks.CarryProp;
using EmergentEchoes.Stages.Testing;
using EmergentEchoes.Utilities.Game;
using EmergentEchoes.Utilities.Game.Enums;
using Godot;
using Godot.Collections;
using NPCProcGen;
using NPCProcGen.Core.Components.Enums;

namespace EmergentEchoes.Entities.Actors
{
	public partial class Actor : CharacterBody2D
	{
		[Signal]
		public delegate void CropPlantedEventHandler(CropMarker2D cropMarker);
		[Signal]
		public delegate void CropHarvestedEventHandler(CropMarker2D cropMarker);

		[Export]
		public int MaxSpeed { get; set; } = 40;
		[Export]
		public int Acceleration { get; set; } = 8;
		[Export]
		public int Friction { get; set; } = 4;

		public WorldLayer WorldLayer { get; set; }
		protected ActorTag2D _actorTag2D;

		protected Sprite2D _seedProp;
		protected CarryProp _carryProp;

		protected AnimationTree _animationTree;
		protected AnimationNodeStateMachinePlayback _animationState;

		protected EmoteController _emoteController;
		protected FloatTextController _floatTextController;

		public override void _Ready()
		{
			_animationTree = GetNode<AnimationTree>("AnimationTree");
			_animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");

			_seedProp = GetNode<Sprite2D>("SeedProp");
			_carryProp = GetNode<CarryProp>("CarryProp");

			_emoteController = GetNode<EmoteController>("EmoteController");
			_floatTextController = GetNode<FloatTextController>("FloatTextController");

			_actorTag2D = GetChildren().OfType<ActorTag2D>().First();
			_actorTag2D.InteractionStarted += OnInteractionStarted;
			_actorTag2D.InteractionEnded += OnInteractionEnded;
			_actorTag2D.EventTriggered += OnEventTriggered;
		}

		protected void SetBlendPositionParams(float x)
		{
			_animationTree.Set("parameters/Idle/blend_position", x);
			_animationTree.Set("parameters/Move/blend_position", x);
			_animationTree.Set("parameters/Attack/blend_position", x);
			_animationTree.Set("parameters/Harvest/blend_position", x);
			_animationTree.Set("parameters/Eat/blend_position", x);
		}

		private void OnInteractionStarted(Variant state, Array<Variant> data)
		{
			ExecuteInteractionStarted();
			FacePartner();
			_emoteController.Activate();
			return;

			void FacePartner()
			{
				var partner = data[0].As<Node2D>();
				Vector2 directionToFace = GlobalPosition.DirectionTo(partner.GlobalPosition);
				_animationTree.Set("parameters/Idle/blend_position", directionToFace.X);
			}
		}

		private void OnInteractionEnded()
		{
			ExecuteInteractionEnded();
			_emoteController.Deactivate();
		}

		protected virtual void ExecuteInteractionStarted() { }
		protected virtual void ExecuteInteractionEnded() { }

		private void OnEventTriggered(Variant eventType, Array<Variant> data)
		{
			EventType type = eventType.As<EventType>();

			switch (type)
			{
				case EventType.CrimeWitnessed:
					_emoteController.ShowEmoteBubble(Emote.Interrobang);
					break;
				case EventType.Detained:
					OnDetained();
					break;
				case EventType.Captured:
					OnCaptured(data);
					break;
			}
		}

		private void OnDetained()
		{
			Visible = false;
		}

		private void OnCaptured(Array<Variant> data)
		{
			Vector2 releaseLocation = data[0].As<Vector2>();
			GlobalPosition = releaseLocation;
			Visible = true;
		}

		protected virtual void ExecuteDetained() { }
		protected virtual void ExecuteCaptured() { }
	}
}