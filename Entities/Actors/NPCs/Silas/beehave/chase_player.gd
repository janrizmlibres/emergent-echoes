extends ActionLeaf

signal move_player(set_state: String, patrol_location: Vector2)
@onready var blackboard_object = $"../../../../Blackboard"

var player_near: bool = false

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "stealing":
		return FAILURE
	
	if blackboard.get_value("player_stolen") == true:
		return FAILURE
	
	if blackboard.get_value("player_near") == true:
		return SUCCESS
	
	move_player.emit(blackboard.get_value("player").get_position())
	return RUNNING 

func _on_steal_area_body_entered(body: Node2D) -> void:
	if body.get_node_or_null("ActorTag2D"):
		blackboard_object.set_value("player_near", true)
	pass # Replace with function body.
