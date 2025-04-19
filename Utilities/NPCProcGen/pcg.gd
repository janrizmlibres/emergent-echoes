extends Node

signal crime_committed(criminal: Actor)
signal danger_detected(source: Actor)

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
	PETITION,
	TALK,
	EAT,
	SHOP,
	THEFT,
	INTERROGATE,
	PURSUIT,
	PLANT,
	HARVEST,
	INTERACT,
	CAUTIOUS,
	FLEE
}

var food_lower_threshold := 5
var food_upper_threshold := 45

func run_evaluation(npc: NPC) -> void:
	WorldState.npc_manager.run_evaluation(npc)

func stop_evaluation(npc: NPC) -> void:
	WorldState.npc_manager.stop_evaluation(npc)

func execute_petition(npc: NPC, target: Actor, resource_type: ResourceType) -> Array:
	return PetitionController.execute(npc, target, resource_type)

func execute_talk(npc: NPC, target: Actor) -> Array[int]:
	return TalkController.execute(npc, target)

func execute_shop(npc: NPC) -> int:
	return ShopController.execute(npc)

func emit_crime_committed(criminal: Actor) -> void:
	crime_committed.emit(criminal)

func emit_danger_detected(source: Actor) -> void:
	danger_detected.emit(source)

func action_to_string(action: Action) -> String:
	match action:
		Action.NONE: return "None"
		Action.PETITION: return "Petition"
		Action.TALK: return "Talk"
		Action.EAT: return "Eat"
		Action.SHOP: return "Shop"
		Action.THEFT: return "Theft"
		Action.INTERROGATE: return "Interrogate"
		Action.PURSUIT: return "Pursuit"
		Action.PLANT: return "Plant"
		Action.HARVEST: return "Harvest"
		Action.INTERACT: return "Interact"
		Action.CAUTIOUS: return "Cautious"
		Action.FLEE: return "Flee"
		_: return "Unknown"
