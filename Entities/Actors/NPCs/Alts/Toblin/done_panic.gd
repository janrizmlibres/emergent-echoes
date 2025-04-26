extends ActionLeaf

@onready var emote_bubble = $"../../../../EmoteBubble"

signal move_actor(set_state: String, patrol_location: Vector2)

func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") != "move to the right":
		return FAILURE
	
	blackboard.set_value("cutscene_state", "go to garreth")
	return SUCCESS
		
	return RUNNING
