extends ConditionLeaf

@onready var seed_prop = $"../../../../SeedProp"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if GameManager.are_there_crops == false:
		seed_prop.visible = true
		blackboard.set_value("current_state", "tending")
		return SUCCESS
	
	return FAILURE
