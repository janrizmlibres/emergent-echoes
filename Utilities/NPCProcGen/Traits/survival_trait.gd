class_name SurvivalTrait
extends BaseTrait

func evaluation_proactive_action():
	add_targetted_action(PCG.Action.PETITION, PCG.ResourceType.MONEY, choose_actor)
	add_targetted_action(PCG.Action.PETITION, PCG.ResourceType.FOOD, choose_actor)

	add_action(PCG.Action.SHOP, PCG.ResourceType.FOOD, {"shop": WorldState.shop})
	add_action(PCG.Action.EAT, PCG.ResourceType.SATIATION)
	add_action(PCG.Action.TALK, PCG.ResourceType.COMPANIONSHIP, {"target": null})

func choose_actor(candidates: Array[Actor]) -> Actor:
	for actor in candidates:
		if WorldState.memory_manager.is_friendly(npc, actor):
			return actor
	
	return candidates[0] if not candidates.is_empty() else null