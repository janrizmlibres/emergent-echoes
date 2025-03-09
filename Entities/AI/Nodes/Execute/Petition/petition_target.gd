@tool
extends ActionLeaf

@export var relationship_increase: int = 3
@export var relationship_decrease: int = -1

@onready var timer: Timer = $Timer

func tick(actor: Node, blackboard: Blackboard) -> int:
	if timer.time_left <= 0:
		blackboard.set_value("action_pending", false)
		return SUCCESS

	var target = blackboard.get_value("target")
	(actor as NPC).face_target(target)
	return RUNNING

func clean_up(npc: NPC, target: Actor, blackboard) -> void:
	npc.emote_bubble.deactivate()
	WorldState.actor_state[npc].is_busy = false
	target.stop_interaction()
	blackboard.set_value("target_secured", false)

func interrupt(actor: Node, blackboard: Blackboard) -> void:
	print(actor.name + " interrupting petition")
	var npc = actor as NPC
	var target = blackboard.get_value("target")
	clean_up(npc, target, blackboard)

func before_run(actor: Node, blackboard: Blackboard) -> void:
	print(actor.name + " starting petition")
	var npc = actor as NPC
	var target: Actor = blackboard.get_value("target")

	npc.emote_bubble.activate()
	WorldState.actor_state[npc].is_busy = true
	target.start_interaction(npc)
	timer.start()

func after_run(actor: Node, blackboard: Blackboard) -> void:
	print(actor.name + " finishing petition")
	var npc = actor as NPC
	var target: Actor = blackboard.get_value("target")
	var resource_type = blackboard.get_value("resource_type")
	clean_up(npc, target, blackboard)

	var relationship = target.memorizer.get_actor_relationship(npc)
	var probability = get_petition_probability(relationship)
	var accepted = randf() <= probability and target.has_resource(resource_type)

	if not accepted:
		npc.memorizer.actor_data[target].relationship += relationship_decrease
		npc.float_text_controller.show_float_text(
			Globals.ResourceType.COMPANIONSHIP,
			int(relationship_decrease),
			true
		)
		return

	var target_resource = target.get_resource(resource_type)
	var deficiency_point = target_resource.get_deficiency_point()
	var amount_to_give = max(1, (target_resource.amount - deficiency_point) * probability)
	target.give_resource(npc, resource_type, amount_to_give)

	target.memorizer.actor_data[npc].last_resource_petitioned = resource_type

	npc.memorizer.actor_data[target].relationship += relationship_increase
	npc.float_text_controller.show_float_text(
		resource_type,
		int(amount_to_give),
		true
	)

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
