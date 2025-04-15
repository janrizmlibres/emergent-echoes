@tool
extends ConditionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var action = blackboard.get_value("action")

	if action == PCG.Action.THEFT:
		WorldState._actor_state[actor as NPC].current_action = PCG.Action.THEFT
		return SUCCESS

	return FAILURE
