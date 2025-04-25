@tool
extends ActionLeaf

func tick(actor: Node, _blackboard: Blackboard) -> int:
	WorldState.npc_manager.end_case(actor)
	PCG.emit_duty_conducted(actor, false)
	actor.set_main_state(NPC.MainState.WANDER)
	return SUCCESS