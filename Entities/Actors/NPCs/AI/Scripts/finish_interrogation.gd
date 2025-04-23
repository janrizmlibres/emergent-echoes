@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var npc = actor as NPC
	var data = blackboard.get_value("data")
	var target = blackboard.get_value("target")
	var case: Crime = data.case

	var relationship = target.memorizer.get_relationship(npc)
	var probability = get_interrogation_probability(relationship)

	if randf() < probability:
		case._verifiers.append(target)
	else:
		case._falsifiers.append(target)

	npc.executor.end_action()
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
