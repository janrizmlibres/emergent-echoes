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
	INTERROGATE,
	PURSUIT,
	PLANT,
	HARVEST
}

enum Emote {
	ELLIPSIS,
	EXCLAMATION,
	HUM
}

static func get_action_enum_string(value: int) -> String:
	match value:
			Action.NONE: return "None"
			Action.THEFT: return "Theft"
			Action.PETITION: return "Petition"
			Action.TALK: return "Talk"
			Action.EAT: return "Eat"
			_: return "Unknown"