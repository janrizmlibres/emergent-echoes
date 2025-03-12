class_name Strategiser

static func evaluation_action(agent: NPC, practice: Globals.SocialPractice) -> Dictionary:
	var candidates: Array[Dictionary] = []

	for trait_mod in agent.traits:
		var action_candidates: Array[Dictionary] = trait_mod.evaluation_action(practice)

		for candidate in action_candidates:
			if randf() <= candidate.weight:
				candidates.append(candidate)

	if candidates.is_empty(): return {}

	var max_weight: float = 0.0
	for candidate in candidates:
		max_weight = max(max_weight, candidate.weight)

	var max_weighted_actions = []
	for candidate in candidates:
		if absf(candidate.weight - max_weight) < 0.01:
			max_weighted_actions.append(candidate)
  
	return max_weighted_actions[randi() % max_weighted_actions.size()].action_data
