@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var target: Actor = blackboard.get_value("data").target
	actor.face_target(target)
	return RUNNING
