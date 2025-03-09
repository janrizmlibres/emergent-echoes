class_name Strategiser

static func evaluation_action(agent: NPC, practice: Globals.SocialPractice) -> Array:
	var evaluations = []

	for trait_mod in agent.traits:
		var actions = trait_mod.evaluation_action(practice)

		for action in actions:
			if randf() <= action[1]:
				evaluations.append([action[0], float("%.2f" % action[1])])

	if evaluations.is_empty(): return []

	var max_weight: float = 0.0
	for action in evaluations:
		max_weight = max(max_weight, action[1])

	var max_weighted_actions = []
	for action in evaluations:
		if abs(action[1] - max_weight) < 0.5:
			max_weighted_actions.append(action)
  
	if not max_weighted_actions.is_empty():
		return max_weighted_actions[randi() % max_weighted_actions.size()][0]
  
	return []
