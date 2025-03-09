@tool
extends ActionLeaf

@export var companionship_increase: int = 20
@export var target_increase: int = 5

@onready var timer: Timer = $Timer

func tick(actor: Node, blackboard: Blackboard) -> int:
	if timer.time_left <= 0:
		blackboard.set_value("action_pending", false)
		return SUCCESS

	var target = blackboard.get_value("target")
	(actor as NPC).face_target(target)
	return RUNNING

func clean_up(npc: NPC, target: Actor) -> void:
	print(npc.name + " cleaning up talk")
	npc.emote_bubble.deactivate()
	WorldState.actor_state[npc].is_busy = false
	target.stop_interaction()

func interrupt(actor: Node, blackboard: Blackboard) -> void:
	var npc = actor as NPC
	var target = blackboard.get_value("target")
	clean_up(npc, target)

func before_run(actor: Node, blackboard: Blackboard) -> void:
	print(actor.name + " starting talk")
	var npc = actor as NPC
	var target: Actor = blackboard.get_value("target")

	npc.emote_bubble.activate()
	WorldState.actor_state[npc].is_busy = true
	target.start_interaction(npc)
	timer.start()

func after_run(actor: Node, blackboard: Blackboard) -> void:
	var npc = actor as NPC
	var target: Actor = blackboard.get_value("target")
	clean_up(npc, target)

	npc.modify_resource(Globals.ResourceType.COMPANIONSHIP, companionship_increase)
	npc.float_text_controller.show_float_text(
		Globals.ResourceType.COMPANIONSHIP,
		String.num_int64(companionship_increase),
		true
	)

	target.modify_resource(Globals.ResourceType.COMPANIONSHIP, target_increase)
	target.float_text_controller.show_float_text(
		Globals.ResourceType.COMPANIONSHIP,
		String.num_int64(target_increase),
		true
	)
