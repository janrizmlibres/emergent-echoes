class_name PetitionController

const RELATIONSHIP_INCREASE := 5
const RELATIONSHIP_DECREASE := 2

static func execute(
	petitioner: Actor,
	target: Actor,
	resource_type: PCG.ResourceType,
	desired_quantity: int
) -> Array:
	var memory_mgr = WorldState.memory_manager
	var resource_mgr = WorldState.resource_manager

	var relationship = memory_mgr.get_relationship(target, petitioner)
	var probability := get_petition_probability(relationship)

	var accepted = randf() <= probability and resource_mgr.holds_resource(
		target,
		resource_type
	)

	if not accepted:
		memory_mgr.modify_relationship(
			petitioner,
			target,
			- RELATIONSHIP_DECREASE
		)
		return [false, 0, -RELATIONSHIP_DECREASE]

	var target_resource = resource_mgr.get_resource(target, resource_type)
	var deficiency_point := target_resource.get_deficiency_point()
	var amount_to_give = (target_resource.amount - deficiency_point) * probability
	amount_to_give = min(desired_quantity, amount_to_give)
	amount_to_give = max(1, amount_to_give)
	resource_mgr.transfer_resource(target, petitioner, resource_type, amount_to_give)

	memory_mgr.modify_relationship(
		petitioner,
		target,
		RELATIONSHIP_INCREASE
	)
	return [true, amount_to_give, RELATIONSHIP_INCREASE]

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
