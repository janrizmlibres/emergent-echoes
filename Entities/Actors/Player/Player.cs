using EmergentEchoes.addons.NPC2DNode;
using EmergentEchoes.Entities.Actors;
using EmergentEchoes.Utilities.Components;
using EmergentEchoes.Utilities.Components.Enums;
using Godot;
using System;
using System.Collections.Generic;

namespace EmergentEchoes
{
	public partial class Player : Actor
	{
		[Export] private int MaxSpeed { get; set; } = 80;
		[Export] private int Acceleration { get; set; } = 10;
		[Export] private int Friction { get; set; } = 10;

		private AnimationTree _animationTree;
		private AnimationNodeStateMachinePlayback _animationState;

		private Dictionary<Actor, float> Relationships { get; set; } = new();

		public override void _Ready()
		{
			_animationTree = GetNode<AnimationTree>("AnimationTree");
			_animationState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");

			AddResources();
		}

		private void AddResources()
		{
			// TODO: Consider adding separate resource class for player
			Resources.Add(new ResourceStat(StatType.Money, 1, true));
			Resources.Add(new ResourceStat(StatType.Food, 1, true));
		}

		public override void AddRelationships(List<Actor> otherActors)
		{
			foreach (Actor actor in otherActors)
			{
				Relationships.Add(actor, 0);
			}
		}

		public override void _PhysicsProcess(double delta)
		{
			Vector2 inputVector = Input.GetVector("left", "right", "up", "down");

			if (inputVector.X != 0)
			{
				_animationTree.Set("parameters/Idle/blend_position", inputVector.X);
				_animationTree.Set("parameters/Move/blend_position", inputVector.X);
				Move(inputVector);
			}
			else if (inputVector.Y != 0)
			{
				Move(inputVector);
			}
			else
			{
				_animationState.Travel("Idle");
				Velocity = Velocity.MoveToward(Vector2.Zero, Friction);
			}

			MoveAndSlide();
		}

		private void Move(Vector2 inputVector)
		{
			_animationState.Travel("Move");
			Velocity = Velocity.MoveToward(inputVector * MaxSpeed, Acceleration);
		}
	}
}