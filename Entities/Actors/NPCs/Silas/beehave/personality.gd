extends Node

@onready var blackboard = $"../Blackboard"
@onready var timer = $"../Timer"
@onready var emote_controller = $"../EmoteController"

func _process(delta: float) -> void:
	if blackboard.get_value("current_state") == "idle":
		blackboard.set_value("current_state", "patrolling")
		
	if blackboard.get_value("is_full") < 10:
		blackboard.set_value("current_state", "eat_food")
		
	if blackboard.get_value("food_inventory") < 4:
		blackboard.set_value("current_state", "buy food")

func _on_npc_alt_detector_body_entered(body: Node2D) -> void:
	if body.get_name() == "GarrethAlt":
		emote_controller.ShowEmoteBubble(10)
		blackboard.set_value("current_state", "going back home")
		return
		
	if blackboard.get_value("current_state") == "chasing":
		return
		
	if body.get_name() == "SilasAlt":
		return
	
	if body.get_node_or_null("ActorTag2D") or body.get_node_or_null("NpcAltDetector"):
		blackboard.set_value("actor", body)
			
		timer.stop()
		emote_controller.ShowEmoteBubble(4)
			
		blackboard.set_value("current_state", "chasing")
	pass # Replace with function body.
