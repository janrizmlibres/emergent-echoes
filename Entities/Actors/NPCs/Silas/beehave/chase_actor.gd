extends ActionLeaf

signal move_player(patrol_location: Vector2)

@onready var blackboard_object = $"../../../../Blackboard"

var player_near: bool = false

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "chasing":
		return FAILURE
	
	if player_near:
		player_near = false
		blackboard.set_value("current_state", "stealing")
		return SUCCESS
	
	move_player.emit(blackboard.get_value("actor").get_position())
	return RUNNING 

func _on_steal_area_body_entered(body: Node2D) -> void:
	if body.get_node_or_null("ActorTag2D"):
		player_near = true
	pass # Replace with function body.
