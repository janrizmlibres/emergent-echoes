class_name ThiefTrait
extends BaseTrait

func evaluation_proactive_action():
	add_targetted_action(
		PCG.Action.THEFT,
		PCG.ResourceType.MONEY,
		choose_actor,
		func(actor): return not actor.is_lawful()
	)

	add_targetted_action(
		PCG.Action.THEFT,
		PCG.ResourceType.FOOD,
		choose_actor,
		func(actor): return not actor.is_lawful()
	)

func choose_actor(candidates: Array[Actor]) -> Actor:
	for actor in candidates:
		if not WorldState.memory_manager.is_trusted(npc, actor):
			return actor
	
	return candidates[0] if not candidates.is_empty() else null