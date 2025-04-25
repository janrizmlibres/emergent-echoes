@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var data = blackboard.get_value("data")

	var relationship = WorldState.memory_manager.get_relationship(data.target, actor)
	var probability = get_interrogation_probability(relationship)

	if randf() < probability:
		data.case.mark_verifier(data.target)
	else:
		data.case.mark_falsifier(data.target)

	if data.case.all_participants_cleared():
		data.case.close_case()

	actor.set_main_state(NPC.MainState.WANDER)
	return SUCCESS

func get_interrogation_probability(relationship_level: float):
	const THRESHOLDS = {
		-26: 0.30,
		-16: 0.50,
		-6: 0.70,
		4: 0.90,
		14: 0.92,
		24: 0.95
	}

	for threshold in THRESHOLDS.keys():
		if relationship_level <= threshold:
			return THRESHOLDS[threshold]

	return 1.00
