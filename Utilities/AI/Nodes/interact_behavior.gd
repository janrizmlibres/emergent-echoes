@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var target: Actor = blackboard.get_value("data").target
	
	if target.is_queued_for_deletion():
		return FAILURE

	actor.face_target(target)
	actor.stop_agent()
	return RUNNING

func interrupt(actor: Node, _blackboard: Blackboard) -> void:
	actor.emote_bubble.deactivate()
	WorldState.set_status(actor, ActorState.State.FREE)

func before_run(actor: Node, _blackboard: Blackboard) -> void:
	actor.emote_bubble.activate()
	WorldState.set_status(actor, ActorState.State.OCCUPIED)
