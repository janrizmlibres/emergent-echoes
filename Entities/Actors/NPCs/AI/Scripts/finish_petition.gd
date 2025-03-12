@tool
extends ActionLeaf

@export var relationship_increase: int = 3
@export var relationship_decrease: int = -1

func tick(actor: Node, blackboard: Blackboard) -> int:
	var npc = actor as NPC
	var target = blackboard.get_value("target")
	var data = blackboard.get_value("data")
	var resource_type = data.resource_type

	var relationship = target.memorizer.get_relationship(npc)
	var probability = get_petition_probability(relationship)

	var accepted = randf() <= probability and target.holds_resource(resource_type)

	if not accepted:
		npc.memorizer.modify_relationship(target, relationship_decrease)
		npc.float_text_controller.show_float_text(
			Globals.ResourceType.COMPANIONSHIP,
			str(relationship_decrease),
			true
		)
	else:
		var target_resource = target.get_resource(resource_type)
		var deficiency_point = target_resource.get_deficiency_point()
		var amount_to_give = max(1, (target_resource.amount - deficiency_point) * probability)
		target.give_resource(npc, resource_type, amount_to_give)

		npc.memorizer.modify_relationship(target, relationship_increase)
		npc.float_text_controller.show_float_text(
			resource_type,
			str(amount_to_give),
			true
		)

	npc.executor.end_action()
	return SUCCESS

func get_petition_probability(relationship_level: float) -> float:
	const THRESHOLDS = {
		-26: 0.05,
		-16: 0.20,
		-6: 0.30,
		4: 0.40,
		14: 0.60,
		24: 0.80
	}
	
	for threshold in THRESHOLDS.keys():
		if relationship_level <= threshold:
			return THRESHOLDS[threshold]

	return 0.95
