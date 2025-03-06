extends ActionLeaf

signal move_actor(patrol_location: Vector2)

@onready var blackboard_object = $"../../../../Blackboard"

var actor_near: bool = false

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "chasing":
		return FAILURE
	
	if actor_near:
		actor_near = false
		blackboard.set_value("current_state", "stealing")
		return SUCCESS
	
	move_actor.emit(blackboard.get_value("actor").get_position())
	return RUNNING 

func _on_steal_area_body_entered(body: Node2D) -> void:
	if body.get_name() == "SilasAlt":
		return
		
	if body.get_node_or_null("SeedProp") == null:
		return
	
	if body.get_name() == blackboard_object.get_value("actor").get_name():
		actor_near = true
	pass # Replace with function body.
