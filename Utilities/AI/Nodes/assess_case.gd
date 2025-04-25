@tool
extends ActionLeaf

@export var duration: float = 10

var timer: float = duration

func tick(_actor: Node, blackboard: Blackboard) -> int:
	timer -= get_process_delta_time()

	if timer <= 0:
		blackboard.set_value("assess_completed", true)
		return SUCCESS

	return RUNNING

func interrupt(actor: Node, _blackboard: Blackboard) -> void:
	actor.emote_bubble.deactivate()

func before_run(actor: Node, _blackboard: Blackboard) -> void:
	timer = duration
	actor.emote_bubble.activate()

func after_run(actor: Node, _blackboard: Blackboard) -> void:
	actor.emote_bubble.deactivate()