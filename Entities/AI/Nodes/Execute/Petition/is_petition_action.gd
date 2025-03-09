@tool
extends ConditionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var action = blackboard.get_value("action")

	if action == Globals.Action.PETITION:
		WorldState.actor_state[actor as NPC].current_action = Globals.Action.PETITION
		return SUCCESS

	return FAILURE
