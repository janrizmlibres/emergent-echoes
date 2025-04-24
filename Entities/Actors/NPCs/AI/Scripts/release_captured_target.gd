@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var npc = actor as NPC
	var data = blackboard.get_value("data")
	var target = blackboard.get_value("target")
	var prison_marker = blackboard.get_value("prison_marker")
	var case: Crime = data.case

	npc.carry_prop.hide_sprite()
	target.global_position = prison_marker.global_position
	target.visible = true

	if target is Player:
		npc.remove_child(target.remote_transform)
		target.add_child(target.remote_transform)
	else:
		(target as NPC).executor.set_enable(true)

	case.status = Crime.Status.SOLVED
	case.complete_investigation(10)

	WorldState.set_captured(target, true)
	npc.executor.end_action()
	return SUCCESS
