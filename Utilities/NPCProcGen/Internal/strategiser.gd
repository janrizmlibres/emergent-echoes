class_name Strategiser

static func evaluation_action(agent: NPC, practice: Globals.SocialPractice) -> Dictionary:
	var candidates: Array[Dictionary] = []

	for trait_mod in agent.traits:
		var action_candidates: Array[Dictionary] = trait_mod.evaluation_action(practice)
		print("Action candidates for ", agent.name, " under ", trait_mod.name, ": ", action_candidates)

		print("Final candidates:")
		for candidate in action_candidates:
			var rand_num = randf()
			print("Random number: ", rand_num, " Candidate weight: ", candidate.weight)
			if rand_num <= candidate.weight:
				print(candidate)
				candidates.append(candidate)

	if candidates.is_empty(): return {}

	var max_weight: float = 0.0
	for candidate in candidates:
		max_weight = max(max_weight, candidate.weight)

	print("Max weight candidates:")
	var max_weighted_actions = []
	for candidate in candidates:
		if absf(candidate.weight - max_weight) < 0.01:
			print(candidate)
			max_weighted_actions.append(candidate)
  
	return max_weighted_actions[randi() % max_weighted_actions.size()].action_data
