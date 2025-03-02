extends ActionLeaf

@onready var animation_tree = $"../../../../AnimationTree"


func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") == "eat food":
		animation_tree.travel("eat")
		blackboard.set_value("current_state", "eating")
		return RUNNING
	
	if blackboard.get_value("current_state") == "eating" && 
	
	return FAILURE
