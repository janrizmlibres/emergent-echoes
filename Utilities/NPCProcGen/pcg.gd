extends Node

enum ResourceType {
	NONE,
	TOTAL_FOOD,
	MONEY,
	FOOD,
	SATIATION,
	COMPANIONSHIP,
	DUTY,
}

enum SocialPractice {
	PROACTIVE,
	REACTIVE
}

enum Action {
	NONE,
	INTERACT,
	THEFT,
	PETITION,
	TALK,
	EAT,
	SHOP,
	INTERROGATE,
	PURSUIT,
	PLANT,
	HARVEST,
	FLEE
}

var food_lower_threshold := 5
var food_upper_threshold := 45

func run_npc(npc: NPC) -> void:
	WorldState.npc_manager.run_npc(npc)