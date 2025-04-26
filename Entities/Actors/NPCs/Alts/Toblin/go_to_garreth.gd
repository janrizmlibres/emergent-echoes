extends ActionLeaf

@onready var emote_bubble = $"../../../../EmoteBubble"

signal move_actor(set_state: String, patrol_location: Vector2)

func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") != "go to garreth":
		return FAILURE
	
	emote_bubble.deactivate()
	GameManager.garreth[0].get_node("Blackboard").set_value("cutscene_state", "toblin is looking for you")
	move_actor.emit(GameManager.garreth[0].global_position)
	
	if _actor.garreth_reached == true:
		_actor.current_location = Vector2.ZERO
		blackboard.set_value("cutscene_state", "report to garreth")
		blackboard.set_value("agent_arrived", false)
		
		_actor.garreth_reached = false
		return SUCCESS
			
	return RUNNING
