extends ActionLeaf

signal move_actor(set_state: String, patrol_location: Vector2)

@onready var emote_bubble = $"../../../../EmoteBubble"

func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") != "go to market":
		return FAILURE
	
	move_actor.emit(Vector2(491, 417))
	
	if blackboard.get_value("agent_arrived") == true:
		_actor.current_location = Vector2.ZERO
		_actor.set_animation_to_idle()
		_actor.face_target(GameManager.toblin[0])
				
		emote_bubble.activate()
		
		blackboard.set_value("agent_arrived", false)
		blackboard.set_value("cutscene_state", "death by being hungry")
		return SUCCESS

	return RUNNING
