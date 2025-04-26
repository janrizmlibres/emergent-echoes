@tool
extends ActionLeaf

@export var duration: float = 15
@export var action := PCG.InteractableAction.PETITION

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
	var data: Dictionary = blackboard.get_value("data")
	data.target.start_interaction(actor, action, data.get("resource_type"))

func after_run(_actor: Node, blackboard: Blackboard) -> void:
	var target: Actor = blackboard.get_value("data").target
	target.stop_interaction()
