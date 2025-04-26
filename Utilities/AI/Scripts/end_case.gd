@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var case: Crime = blackboard.get_value("data").case
	var increase := 30 if case.is_solved() else -1

	WorldState.resource_manager.modify_resource(actor, PCG.ResourceType.DUTY, increase)
	WorldState.npc_manager.end_case(actor)

	PCG.emit_duty_conducted(actor, case.is_solved())
	actor.set_main_state(NPC.MainState.WANDER)
	return SUCCESS