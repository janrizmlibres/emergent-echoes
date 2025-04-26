extends ActionLeaf

@onready var emote_bubble = $"../../../../EmoteBubble"

var frantic_times = 0
var frantic = false

signal move_actor(set_state: String, patrol_location: Vector2)

func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") != "move to the left":
		return FAILURE
		
	move_actor.emit(Vector2(GameManager.farmer_death_location.x - 10, _actor.global_position.y))
	
	if blackboard.get_value("agent_arrived") == true:
		_actor.set_animation_to_idle()
		_actor.current_location = Vector2.ZERO
		blackboard.set_value("cutscene_state", "move to the right")
		blackboard.set_value("agent_arrived", false)
		return SUCCESS
		
	return RUNNING
