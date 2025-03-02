using System;
using System.Linq;
using System.Text;
using Godot;
using NPCProcGen;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Traits;

namespace EmergentEchoes.Entities.Debugging.DebugLabel
{
	public partial class DebugLabel : Label
	{
		[Export]
		public ActorTag2D Actor { get; set; }

		public override void _Process(double delta)
		{
			UpdateLabel();
		}

		private void UpdateLabel()
		{
			StringBuilder sb = new();
			ResourceManager resMgr = ResourceManager.Instance;

			if (resMgr.HasResource(ResourceType.Money, Actor))
			{
				float value = resMgr.GetResourceAmount(ResourceType.Money, Actor);
				sb.AppendLine($"Money: {value}");
			}

			if (resMgr.HasResource(ResourceType.Food, Actor))
			{
				sb.AppendLine($"Food: {resMgr.GetResourceAmount(ResourceType.Food, Actor)}");
			}

			if (resMgr.HasResource(ResourceType.Satiation, Actor))
			{
				float value = resMgr.GetResourceAmount(ResourceType.Satiation, Actor);
				sb.AppendLine($"Satiation: {Mathf.Round(value)}");
			}

			if (resMgr.HasResource(ResourceType.Companionship, Actor))
			{
				float value = resMgr.GetResourceAmount(ResourceType.Companionship, Actor);
				sb.AppendLine($"Companionship: {Mathf.Round(value)}");
			}

			if (Actor is NPCAgent2D npc)
			{
				if (npc.Traits.Any(t => t is LawfulTrait))
				{
					float value = resMgr.GetResourceAmount(ResourceType.Duty, Actor);
					sb.AppendLine($"Duty: {Mathf.Round(value)}");
				}
			}

			Text = sb.ToString().TrimEnd();
		}
	}
}