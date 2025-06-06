extends ActionLeaf

signal move_actor(patrol_location: Vector2)

@onready var blackboard_object = $"../../../../Blackboard"
@onready var emote_bubble = $"../../../../EmoteBubble"

@export var max_distance: float = 150.0

var actor_near = false

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "chasing":
		return FAILURE
		
	if actor.position.distance_to(blackboard.get_value("last_patrol_location")) > max_distance:
		blackboard.set_value("current_state", "done petitioning")
		return FAILURE
		
	if actor_near:
		emote_bubble.show_emote_bubble(Globals.Emote.EXCLAMATION)
		
		blackboard.set_value("current_state", "petitioning")
		actor_near = false
		return SUCCESS
		
	move_actor.emit(blackboard.get_value("actor").get_position())
	return RUNNING 

func _on_petition_area_body_entered(body: Node2D) -> void:
	if blackboard_object.get_value("current_state") != "chasing":
		return
		
	if body.get_name() == "GarrethAlt":
		return
		
	if body.get_node_or_null("SeedProp") == null:
		return
	
	if body.get_name() == blackboard_object.get_value("actor").get_name():
		actor_near = true
	pass # Replace with function body.
