extends ActionLeaf

@onready var emote_bubble = $"../../../../EmoteBubble"

signal move_actor(set_state: String, patrol_location: Vector2)

func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") != "inspecting the farmer":
		return FAILURE
	
	move_actor.emit(GameManager.farmer_death_location)
	
	if blackboard.get_value("agent_arrived") == true:
		_actor.set_animation_to_idle()
		_actor.current_location = Vector2.ZERO
		emote_bubble.activate()
		blackboard.set_value("cutscene_state", "go to garreth")
		blackboard.set_value("agent_arrived", false)
		return SUCCESS
			
	return RUNNING
