using System.Collections.Generic;
using Godot;

namespace EmergentEchoes.Stages.Testing
{
	public partial class WorldLayer : TileMapLayer
	{
		public List<Node2D> Obstacles { get; set; } = new();

		private readonly Dictionary<Vector2I, NavigationPolygon> _removedNavMeshes = new();

		public override bool _UseTileDataRuntimeUpdate(Vector2I coords)
		{
			foreach (Node2D obstacle in Obstacles)
			{
				Vector2 localPos = ToLocal(obstacle.GlobalPosition);
				Vector2I currentCoord = LocalToMap(localPos);

				if (currentCoord.Equals(coords))
				{
					return true;
				}
			}

			return _removedNavMeshes.ContainsKey(coords);
		}

		public override void _TileDataRuntimeUpdate(Vector2I coords, TileData tileData)
		{
			if (_removedNavMeshes.ContainsKey(coords))
			{
				foreach (Node2D obstacle in Obstacles)
				{
					Vector2 localPos = ToLocal(obstacle.GlobalPosition);
					Vector2I currentCoord = LocalToMap(localPos);

					if (currentCoord.Equals(coords))
					{
						tileData.SetNavigationPolygon(0, null);
						return;
					}
				}

				tileData.SetNavigationPolygon(0, _removedNavMeshes[coords]);
				_removedNavMeshes.Remove(coords);
			}
			else
			{
				_removedNavMeshes.Add(coords, tileData.GetNavigationPolygon(0));
				tileData.SetNavigationPolygon(0, null);
			}
		}
	}
}