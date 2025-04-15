@tool
extends ActionLeaf

@export var companionship_increase: int = 20
@export var target_increase: int = 5
@export var relationship_increase: int = 1

func tick(actor: Node, blackboard: Blackboard) -> int:
	var npc = actor as NPC
	var target = blackboard.get_value("target")

	npc.modify_resource(PCG.ResourceType.COMPANIONSHIP, companionship_increase)
	npc.memorizer.modify_relationship(target, relationship_increase)
	npc.float_text_controller.show_float_text(
		PCG.ResourceType.COMPANIONSHIP,
		String.num_int64(companionship_increase),
		true
	)

	target.modify_resource(PCG.ResourceType.COMPANIONSHIP, target_increase)
	target.memorizer.modify_relationship(npc, relationship_increase)
	target.float_text_controller.show_float_text(
		PCG.ResourceType.COMPANIONSHIP,
		String.num_int64(target_increase),
		true
	)

	npc.executor.end_action()
	return SUCCESS
