@tool
extends ActionLeaf

@export var duration: float = 15

var timer: float = duration

func tick(_actor: Node, _blackboard: Blackboard) -> int:
	timer -= get_process_delta_time()

	if timer <= 0:
		return SUCCESS

	return RUNNING

func interrupt(_actor: Node, blackboard: Blackboard) -> void:
	var target = blackboard.get_value("target")
	if target == null: return
	target.stop_interaction()

func before_run(actor: Node, blackboard: Blackboard) -> void:
	timer = duration
	var npc = actor as NPC
	var target = blackboard.get_value("target")
	target.start_interaction(npc)

func after_run(_actor: Node, blackboard: Blackboard) -> void:
	var target = blackboard.get_value("target")
	target.stop_interaction()
