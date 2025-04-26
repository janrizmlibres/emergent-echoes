extends Node

signal crime_committed(crime: Crime)
signal threat_present(source: Actor, recipient: Actor)
signal duty_conducted(actor: Actor, is_success: bool)
signal satiation_depleted(actor: Actor)

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
	WANDER,
	PETITION,
	TALK,
	EAT,
	SHOP,
	THEFT,
	INTERROGATE,
	PURSUIT,
	ASSESS,
	PLANT,
	HARVEST,
	FLEE,
	CAUTIOUS,
	INTERACT,
}

var food_lower_threshold := 5
var food_upper_threshold := 45

func run_evaluation(npc: NPC) -> void:
	WorldState.npc_manager.evaluators[npc].start_timer()

func stop_evaluation(npc: NPC) -> void:
	WorldState.npc_manager.evaluators[npc].stop_timer()

func execute_petition(
	petitioner: Actor,
	target: Actor,
	resource_type,
	desired_quantity := Globals.INT_MAX
) -> Array:
	return PetitionController.execute(petitioner, target, resource_type, desired_quantity)

func execute_talk(npc: NPC, target: Actor) -> Array[int]:
	return TalkController.execute(npc, target)

func execute_shop(npc: NPC) -> int:
	return ShopController.execute(npc)

func execute_theft(npc: NPC, target: Actor, resource_type: ResourceType) -> int:
	return TheftController.execute(npc, target, resource_type)

func emit_crime_committed(crime: Crime) -> void:
	crime_committed.emit(crime)

func emit_threat_present(source: Actor, recipient: Actor) -> void:
	threat_present.emit(source, recipient)

func emit_duty_conducted(actor: Actor, is_success: bool) -> void:
	duty_conducted.emit(actor, is_success)

func emit_satiation_depleted(actor: Actor) -> void:
	satiation_depleted.emit(actor)

func resource_to_string(type: ResourceType) -> String:
	match type:
		ResourceType.NONE: return "None"
		ResourceType.TOTAL_FOOD: return "Total Food"
		ResourceType.MONEY: return "Money"
		ResourceType.FOOD: return "Food"
		ResourceType.SATIATION: return "Satiation"
		ResourceType.COMPANIONSHIP: return "Companionship"
		ResourceType.DUTY: return "Duty"
		_: return "Unknown"

func action_to_string(action: Action) -> String:
	match action:
		Action.WANDER: return "Wander"
		Action.PETITION: return "Petition"
		Action.TALK: return "Talk"
		Action.EAT: return "Eat"
		Action.SHOP: return "Shop"
		Action.THEFT: return "Theft"
		Action.INTERROGATE: return "Interrogate"
		Action.PURSUIT: return "Pursuit"
		Action.ASSESS: return "Assess"
		Action.PLANT: return "Plant"
		Action.HARVEST: return "Harvest"
		Action.INTERACT: return "Interact"
		Action.CAUTIOUS: return "Cautious"
		Action.FLEE: return "Flee"
		_: return "Unknown"
