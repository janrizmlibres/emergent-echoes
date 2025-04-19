class_name PetitionController

const RELATIONSHIP_INCREASE := 3
const RELATIONSHIP_DECREASE := 1

static func execute(npc: NPC, target: Actor, resource_type: PCG.ResourceType) -> Array:
	WorldState.npc_manager.execute_petition(npc, target, resource_type)
	var relationship = target.memorizer.get_relationship(npc)
	var probability = get_petition_probability(relationship)

	var accepted = randf() <= probability and target.holds_resource(resource_type)

	if not accepted:
		WorldState.memory_manager.modify_relationship(
			npc,
			target,
			(-RELATIONSHIP_DECREASE)
		)
		return [false, 0]

	var target_resource = target.get_resource(resource_type)
	var deficiency_point = target_resource.get_deficiency_point()
	var amount_to_give = max(1, (target_resource.amount - deficiency_point) * probability)
	target.give_resource(npc, resource_type, amount_to_give)

	WorldState.memory_manager.modify_relationship(
		npc,
		target,
		RELATIONSHIP_INCREASE
	)
	return [true, amount_to_give]

static func get_petition_probability(relationship_level: float) -> float:
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