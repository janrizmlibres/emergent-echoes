@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var target = blackboard.get_value("target")
	(actor as NPC).face_target(target)
	return RUNNING

func interrupt(actor: Node, _blackboard: Blackboard) -> void:
	var npc = actor as NPC
	npc.emote_bubble.deactivate()
	WorldState.set_is_busy(npc, false)

func before_run(actor: Node, _blackboard: Blackboard) -> void:
	var npc = actor as NPC
	npc.emote_bubble.activate()
	WorldState.set_is_busy(npc, true)
