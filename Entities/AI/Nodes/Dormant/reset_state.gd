@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var actor_node: Actor = actor as Actor

	WorldState.actor_state[actor_node].current_action = Globals.Action.NONE
	WorldState.actor_state[actor_node].current_petition_resource = Globals.ResourceType.NONE
	WorldState.actor_state[actor_node].is_busy = false

	blackboard.set_value("interaction required", false)
	blackboard.set_value("action_pending", false)
	blackboard.set_value("target_found", false)
	blackboard.set_value("target_secured", false)
	blackboard.set_value("eat_finished", false)
	(actor_node as NPC).start_timer()
	return SUCCESS
