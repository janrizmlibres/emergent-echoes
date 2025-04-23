class_name TalkController

const RELATIONSHIP_INCREASE := 1
const SELF_COMPANION_INCREASE := 20
const TARGET_COMPANION_INCREASE := 5

static func execute(npc: NPC, target: Actor) -> Array[int]:
	WorldState.resource_manager.modify_resource(
		npc,
		PCG.ResourceType.COMPANIONSHIP,
		SELF_COMPANION_INCREASE
	)
	WorldState.memory_manager.modify_relationship(npc, target, RELATIONSHIP_INCREASE)

	WorldState.resource_manager.modify_resource(
		target,
		PCG.ResourceType.COMPANIONSHIP,
		TARGET_COMPANION_INCREASE
	)
	WorldState.memory_manager.modify_relationship(target, npc, RELATIONSHIP_INCREASE)
	return [SELF_COMPANION_INCREASE, TARGET_COMPANION_INCREASE]
