extends Node

@onready var blackboard = $"../Blackboard"
@onready var timer = $"../Timer"

func _on_npc_alt_detector_body_entered(body: Node2D) -> void:
	if blackboard.get_value("current_state") == "stealing":
		return
		
	if body.get_name() == "SilasAlt":
		return
	
	if body.get_name() == "GarrethAlt":
		return
	
	if body.get_node_or_null("ActorTag2D") or body.get_node_or_null("NpcAltDetector"):
		timer.stop()
		blackboard.set_value("is_idle", false)
		blackboard.set_value("actor", body)
		blackboard.set_value("actor_found", true)
	pass # Replace with function body.
