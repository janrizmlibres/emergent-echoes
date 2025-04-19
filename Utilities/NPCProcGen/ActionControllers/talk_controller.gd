class_name TalkController

const RELATIONSHIP_INCREASE := 1
const SELF_COMPANION_INCREASE := 20
const TARGET_COMPANION_INCREASE := 5

static func execute(npc: NPC, target: Actor) -> Array[int]:
	npc.modify_resource(PCG.ResourceType.COMPANIONSHIP, SELF_COMPANION_INCREASE)
	npc.memorizer.modify_relationship(target, RELATIONSHIP_INCREASE)

	target.modify_resource(PCG.ResourceType.COMPANIONSHIP, TARGET_COMPANION_INCREASE)
	target.memorizer.modify_relationship(npc, RELATIONSHIP_INCREASE)
	return [SELF_COMPANION_INCREASE, TARGET_COMPANION_INCREASE]