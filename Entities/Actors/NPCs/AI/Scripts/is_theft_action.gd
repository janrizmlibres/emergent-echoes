@tool
extends ConditionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var action = blackboard.get_value("action")

	if action == Globals.Action.THEFT:
		WorldState.actor_state[actor as NPC].current_action = Globals.Action.THEFT
		return SUCCESS

	return FAILURE
