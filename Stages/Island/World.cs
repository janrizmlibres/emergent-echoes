using System.Collections.Generic;
using System.Linq;
using EmergentEchoes.Entities.Actors;
using EmergentEchoes.Entities.Objects.Crop;
using EmergentEchoes.Stages.Testing;
using Godot;
using Godot.Collections;
using NPCProcGen;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Internal;

namespace EmergentEchoes.Stages.Island
{
	public partial class World : Node2D
	{
		private readonly List<Crop> _crops = new();

		private PackedScene cropScene = ResourceLoader.Load<PackedScene>(
			"res://Entities/Objects/Crop/crop.tscn"
		);

		public override void _Ready()
		{
			AutoloadInitializer.Init(this);

			WorldLayer worldLayer = GetNode<WorldLayer>("WorldLayer");
			Array<Vector2I> validTiles = GetValidTilePositions(worldLayer);

			List<Actor> actors = GetTree().GetNodesInGroup("Actors").OfType<Actor>().ToList();
			actors.ForEach(actor =>
			{
				actor.CropPlanted += OnCropPlanted;
				actor.CropHarvested += OnCropHarvested;

				actor.WorldLayer = worldLayer;
				(actor as NPC)?.ValidTilePositions.AddRange(validTiles);
			});
		}

		private static Array<Vector2I> GetValidTilePositions(WorldLayer worldLayer)
		{
			Array<Vector2I> validTilePositions = new();
			Array<Vector2I> usedCells = worldLayer.GetUsedCells();

			foreach (Vector2I cell in usedCells)
			{
				TileData tileData = worldLayer.GetCellTileData(cell);

				if (tileData != null && (bool)tileData.GetCustomData("isNavigatable"))
				{
					validTilePositions.Add(cell);
				}
			}

			return validTilePositions;
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
