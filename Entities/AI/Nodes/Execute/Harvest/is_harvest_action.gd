@tool
extends ConditionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var action = blackboard.get_value("action")

	if action == Globals.Action.HARVEST:
		WorldState.actor_state[actor as NPC].current_action = Globals.Action.HARVEST
		return SUCCESS

	return FAILURE
