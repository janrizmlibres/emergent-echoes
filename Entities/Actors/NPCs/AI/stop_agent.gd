@tool
extends ActionLeaf

func tick(actor: Node, _blackboard: Blackboard) -> int:
	var has_stopped = actor.stop_agent()
	actor.animation_state.travel("Idle")
	return RUNNING if not has_stopped else SUCCESS
