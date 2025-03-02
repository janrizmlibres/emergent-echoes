using System.Collections.Generic;
using System.Linq;
using EmergentEchoes.Entities.Actors;
using EmergentEchoes.Entities.Objects.Crop;
using Godot;
using NPCProcGen;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Internal;

namespace EmergentEchoes.Stages.Island
{
	public partial class World : Node2D
	{
		private readonly List<Crop> _crops = new();

		private PackedScene cropScene = GD.Load<PackedScene>("res://Entities/Objects/Crop/crop.tscn");

		public override void _Ready()
		{
			AutoloadInitializer.Init(this);

			List<Actor> actors = GetTree().GetNodesInGroup("Actors").OfType<Actor>().ToList();
			actors.ForEach(npc =>
			{
				npc.CropPlanted += OnCropPlanted;
				npc.CropHarvested += OnCropHarvested;
			});
		}

		public override void _Process(double delta)
		{
			Sensor.Update(delta);
			_crops.ForEach(crop => crop.Update());
		}

		private void OnCropPlanted(CropMarker2D cropMarker)
		{
			Crop crop = cropScene.Instantiate<Crop>();
			cropMarker.AddChild(crop);
			_crops.Add(crop);
		}

		private void OnCropHarvested(CropMarker2D cropMarker)
		{
			Crop crop = cropMarker.GetChild<Crop>(0);
			crop.QueueFree();
		}
	}
}
