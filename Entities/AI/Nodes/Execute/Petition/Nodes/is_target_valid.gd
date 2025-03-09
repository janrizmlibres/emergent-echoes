@tool
extends ConditionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var target = blackboard.get_value("target")

	if WorldState.is_actor_valid_target(actor as NPC, target):
		return SUCCESS

	blackboard.set_value("action_pending", false)
	return FAILURE