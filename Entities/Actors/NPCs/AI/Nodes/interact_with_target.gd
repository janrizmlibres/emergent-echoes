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
	var target: Actor = blackboard.get_value("data").target
	if target.is_queued_for_deletion(): return
	target.stop_interaction()

func before_run(actor: Node, blackboard: Blackboard) -> void:
	var target: Actor = blackboard.get_value("data").target
	target.start_interaction(actor)

func after_run(_actor: Node, blackboard: Blackboard) -> void:
	var target: Actor = blackboard.get_value("data").target
	target.stop_interaction()
