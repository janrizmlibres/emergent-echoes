@tool
extends ActionLeaf

@export var duration: float = 10

var timer: float = duration

func tick(_actor: Node, blackboard: Blackboard) -> int:
	timer -= get_process_delta_time()

	if timer <= 0:
		var data = blackboard.get_value("data")
		data.assess_completed = true
		blackboard.set_value("data", data)
		return SUCCESS

	return RUNNING

func interrupt(actor: Node, _blackboard: Blackboard) -> void:
	(actor as NPC).emote_bubble.deactivate()

func before_run(actor: Node, _blackboard: Blackboard) -> void:
	timer = duration
	(actor as NPC).emote_bubble.activate()

func after_run(actor: Node, _blackboard: Blackboard) -> void:
	(actor as NPC).emote_bubble.deactivate()
