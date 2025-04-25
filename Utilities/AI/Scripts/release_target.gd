@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var data = blackboard.get_value("data")
	var prison: Prison = blackboard.get_value("prison")

	data.target.handle_captivity(actor, prison)
	WorldState.set_status(data.target, ActorState.State.CAPTURED)
	WorldState.npc_manager.end_case(actor)
	PCG.emit_duty_conducted(actor, true)

	if not data.is_reactive:
		actor.set_main_state(NPC.MainState.WANDER)
	else:
		actor.set_react_state(NPC.ReactState.NONE)

	return SUCCESS
