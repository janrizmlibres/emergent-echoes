extends ActionLeaf

@onready var emote_bubble = $"../../../../EmoteBubble"

var frantic_times = 0
var frantic = false

signal move_actor(set_state: String, patrol_location: Vector2)

func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") != "look to the left":
		return FAILURE
		
	_actor.animation_tree.set("parameters/Idle/blend_position", "left")
	blackboard.set_value("cutscene_state", "finished being interrogated by garreth")
	return SUCCESS
