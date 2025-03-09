class_name SurvivalTrait
extends BaseTrait

func _evaluation_proactive_action():
	for resource_type in ResourceStat.TANGIBLE_TYPES:
		var actor_candidates = get_actor_candidates(resource_type)

		actor_candidates = actor_candidates.filter(func(actor):
			return actor_node.memorizer.is_valid_petition_target(actor, resource_type)
		)

		var target_actor = choose_actor(actor_candidates)
		if (target_actor == null): continue
		add_action(Globals.Action.PETITION, resource_type, [target_actor, resource_type])

	if actor_node.has_resource(Globals.ResourceType.FOOD):
		add_action(Globals.Action.EAT, Globals.ResourceType.SATIATION)

	add_action(Globals.Action.TALK, Globals.ResourceType.COMPANIONSHIP)

func choose_actor(candidates: Array[Actor]) -> Actor:
	for actor in candidates:
		if actor_node.memorizer.is_friendly(actor):
			return actor
	
	return candidates[0] if not candidates.is_empty() else null
