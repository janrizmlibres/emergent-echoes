extends Node

@onready var blackboard = $"../Blackboard"

func _process(delta: float) -> void:
	if blackboard.get_value("current_state") == "idle":
		blackboard.set_value("current_state", "patrolling")

func _on_npc_alt_detector_body_entered(body: Node2D) -> void:
	if blackboard.get_value("current_state") == "petitioning":
		return

	if body.get_name() == "SilasAlt":
		return
	
	if body.get_name() == "GarrethAlt":
		return

	if body.get_node_or_null("ActorTag2D") or body.get_node_or_null("NpcAltDetector"):
		blackboard.set_value("actor", body)
		blackboard.set_value("current_state", "petitioning")
	pass # Replace with function body.
