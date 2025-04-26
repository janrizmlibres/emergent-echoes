@tool
extends ActionLeaf

@export var is_reactive := false

func tick(actor: Node, _blackboard: Blackboard) -> int:
	if actor == null:
		return FAILURE

	if not is_reactive:
		actor.set_main_state(NPC.MainState.WANDER)
	else:
		actor.set_react_state(NPC.ReactState.NONE)
		
	return SUCCESS
