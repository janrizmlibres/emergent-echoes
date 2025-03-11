@tool
extends ConditionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var npc = actor as NPC
	var target = blackboard.get_value("target")

	if WorldState.is_actor_valid_target(npc, target):
		return SUCCESS

	npc.executor.end_action()
	return FAILURE
