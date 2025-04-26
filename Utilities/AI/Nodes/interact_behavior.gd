@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var target = blackboard.get_value("data").get("target")
	
	if target == null:
		return FAILURE

	actor.face_target(target)
	actor.idle_state()
	return RUNNING

func interrupt(actor: Node, _blackboard: Blackboard) -> void:
	actor.emote_bubble.deactivate()
	WorldState.set_status(actor, ActorState.State.FREE)

func before_run(actor: Node, _blackboard: Blackboard) -> void:
	actor.emote_bubble.activate()
	WorldState.set_status(actor, ActorState.State.OCCUPIED)
