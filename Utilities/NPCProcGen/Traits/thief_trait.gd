class_name ThiefTrait
extends BaseTrait

func evaluation_proactive_action():
	for resource_type in ResourceStat.TANGIBLE_TYPES:
		var actor_candidates = get_actor_candidates(resource_type)
		actor_candidates = actor_candidates.filter(func(actor): return not actor.is_lawful())

		var target_actor = choose_actor(actor_candidates)
		if (target_actor == null): continue

		add_action(Globals.Action.THEFT, resource_type, {
			"target": target_actor,
			"resource_type": resource_type
		})

func choose_actor(candidates: Array[Actor]) -> Actor:
	for actor in candidates:
		if not actor_node.memorizer.is_trusted(actor):
			return actor
	
	return candidates[0] if not candidates.is_empty() else null