@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var npc = actor as NPC

	var move_position: Vector2 = blackboard.get_value("move_position")
	npc.navigation_agent.target_position = move_position

	if (npc.navigation_agent.is_navigation_finished()):
		return SUCCESS
	
	npc.move_agent()
	return RUNNING
