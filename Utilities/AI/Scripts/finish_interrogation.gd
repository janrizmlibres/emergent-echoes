@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var data = blackboard.get_value("data")

	var relationship = WorldState.memory_manager.get_relationship(data.target, actor)
	var probability = get_interrogation_probability(relationship)

	var is_success = randf() < probability
	
	if is_success:
		data.case.mark_verifier(data.target)
	else:
		data.case.mark_falsifier(data.target)
	
	WorldState.resource_manager.modify_resource(
		actor,
		PCG.ResourceType.DUTY,
		30 if is_success else -1
	)
	PCG.emit_duty_conducted(actor, is_success)

	actor.set_main_state(NPC.MainState.WANDER)
	return SUCCESS

func get_interrogation_probability(relationship_level: float):
	const THRESHOLDS = {
		10: 0.50,
		20: 0.70,
		30: 0.90,
	}

	for threshold in THRESHOLDS.keys():
		if relationship_level <= threshold:
			return THRESHOLDS[threshold]

	return 1.00
