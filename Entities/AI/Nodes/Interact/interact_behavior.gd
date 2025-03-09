@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var npc = actor as NPC
	var target = blackboard.get_value("interaction_target")
	npc.face_target(target)
	return RUNNING

func interrupt(actor: Node, _blackboard: Blackboard) -> void:
	print(actor.name + " interrupting interact")
	var npc = actor as NPC
	(npc).emote_bubble.deactivate()
	WorldState.actor_state[npc].is_busy = false

func before_run(actor: Node, _blackboard: Blackboard) -> void:
	print(actor.name + " starting interact")
	var npc = actor as NPC
	npc.emote_bubble.activate()
	WorldState.actor_state[npc].is_busy = true
