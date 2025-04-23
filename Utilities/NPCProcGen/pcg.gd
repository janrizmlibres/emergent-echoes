extends Node

signal crime_committed(crime: Crime)
signal danger_occured(source: Actor)

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
	PLANT,
	HARVEST,
	PURSUIT_REACT,
	INTERACT,
	CAUTIOUS,
	FLEE
}

var food_lower_threshold := 5
var food_upper_threshold := 45

func run_evaluation(npc: NPC) -> void:
	WorldState.npc_manager.evaluators[npc].start_timer()

func stop_evaluation(npc: NPC) -> void:
	WorldState.npc_manager.evaluators[npc].stop_timer()

func execute_petition(npc: NPC, target: Actor, resource_type: ResourceType) -> Array:
	return PetitionController.execute(npc, target, resource_type)

func execute_talk(npc: NPC, target: Actor) -> Array[int]:
	return TalkController.execute(npc, target)

func execute_shop(npc: NPC) -> int:
	return ShopController.execute(npc)

func execute_theft(npc: NPC, target: Actor, resource_type: ResourceType) -> int:
	return TheftController.execute(npc, target, resource_type)

func emit_crime_committed(criminal: Actor) -> void:
	crime_committed.emit(criminal)

func emit_danger_detected(source: Actor) -> void:
	danger_occured.emit(source)

func is_action_reactive(action: Action) -> bool:
	var reactive_actions = [
		Action.PURSUIT_REACT,
		Action.INTERACT,
		Action.CAUTIOUS,
		Action.FLEE,
	]

	if reactive_actions.has(action):
		return true

	return false

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
		Action.PLANT: return "Plant"
		Action.HARVEST: return "Harvest"
		Action.INTERACT: return "Interact"
		Action.CAUTIOUS: return "Cautious"
		Action.FLEE: return "Flee"
		_: return "Unknown"
	
func map_action_to_main_state(action: Action) -> NPC.MainState:
	match action:
		Action.WANDER:
			return NPC.MainState.WANDER
		Action.PETITION:
			return NPC.MainState.PETITION
		Action.TALK:
			return NPC.MainState.TALK
		Action.EAT:
			return NPC.MainState.EAT
		Action.SHOP:
			return NPC.MainState.SHOP
		Action.THEFT:
			return NPC.MainState.THEFT
		Action.INTERROGATE:
			return NPC.MainState.INTERROGATE
		Action.PURSUIT:
			return NPC.MainState.PURSUIT
		Action.PLANT:
			return NPC.MainState.PLANT
		Action.HARVEST:
			return NPC.MainState.HARVEST
		_:
			print_debug("Invalid action: ", action)
			return NPC.MainState.WANDER

func map_main_state_to_action(main_state: NPC.MainState) -> PCG.Action:
	match main_state:
		NPC.MainState.WANDER:
			return PCG.Action.WANDER
		NPC.MainState.PETITION:
			return PCG.Action.PETITION
		NPC.MainState.TALK:
			return PCG.Action.TALK
		NPC.MainState.EAT:
			return PCG.Action.EAT
		NPC.MainState.SHOP:
			return PCG.Action.SHOP
		NPC.MainState.THEFT:
			return PCG.Action.THEFT
		NPC.MainState.INTERROGATE:
			return PCG.Action.INTERROGATE
		NPC.MainState.PURSUIT:
			return PCG.Action.PURSUIT
		NPC.MainState.PLANT:
			return PCG.Action.PLANT
		NPC.MainState.HARVEST:
			return PCG.Action.HARVEST
		_:
			print_debug("Invalid main state: ", main_state)
			return PCG.Action.WANDER

func map_react_state_to_action(react_state: NPC.ReactState) -> PCG.Action:
	match react_state:
		NPC.ReactState.PURSUIT:
			return PCG.Action.PURSUIT_REACT
		NPC.ReactState.INTERACT:
			return PCG.Action.INTERACT
		NPC.ReactState.CAUTIOUS:
			return PCG.Action.CAUTIOUS
		NPC.ReactState.FLEE:
			return PCG.Action.FLEE
		_:
			print_debug("Invalid react state: ", react_state)
			return PCG.Action.WANDER
