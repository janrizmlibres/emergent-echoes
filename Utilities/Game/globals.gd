class_name Globals

const INT_MAX = 9223372036854775807

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

enum Emote {
	ELLIPSIS,
	EXCLAMATION,
	HUM
}

static func get_action_string(value: int) -> String:
	match value:
			Action.NONE: return "None"
			Action.INTERACT: return "Interact"
			Action.THEFT: return "Theft"
			Action.PETITION: return "Petition"
			Action.TALK: return "Talk"
			Action.EAT: return "Eat"
			Action.SHOP: return "Shop"
			Action.INTERROGATE: return "Interrogate"
			Action.PURSUIT: return "Pursuit"
			Action.PLANT: return "Plant"
			Action.HARVEST: return "Harvest"
			Action.FLEE: return "Flee"
			_: return "Unknown"

static func get_resource_string(value: int) -> String:
	match value:
			ResourceType.NONE: return "None"
			ResourceType.TOTAL_FOOD: return "Total Food"
			ResourceType.MONEY: return "Money"
			ResourceType.FOOD: return "Food"
			ResourceType.SATIATION: return "Satiation"
			ResourceType.COMPANIONSHIP: return "Companionship"
			ResourceType.DUTY: return "Duty"
			_: return "Unknown"