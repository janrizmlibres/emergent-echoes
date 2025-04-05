class_name SurvivalTrait
extends BaseTrait

func evaluation_proactive_action():
	for resource_type in ResourceStat.TANGIBLE_TYPES:
		var actor_candidates = get_actor_candidates(resource_type)

		var target_actor = choose_actor(actor_candidates)
		if (target_actor == null): continue

		add_action(Globals.Action.PETITION, resource_type, {
			"target": target_actor,
			"resource_type": resource_type
		})
		
	add_action(Globals.Action.TALK, Globals.ResourceType.COMPANIONSHIP)

	if actor_node.holds_resource(Globals.ResourceType.FOOD):
		add_action(Globals.Action.EAT, Globals.ResourceType.SATIATION)
	
	if actor_node.get_resource_amount(Globals.ResourceType.MONEY) > 10 \
		and WorldState.shop.food_amount > 0:
		add_action(Globals.Action.SHOP, Globals.ResourceType.SATIATION)

func choose_actor(candidates: Array[Actor]) -> Actor:
	for actor in candidates:
		if actor_node.memorizer.is_friendly(actor):
			return actor
	
	return candidates[0] if not candidates.is_empty() else null
