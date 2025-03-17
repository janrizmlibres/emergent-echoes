extends Node

@onready var blackboard = $"../Blackboard"
@onready var timer = $"../Timer"
@onready var emote_bubble = $"../EmoteBubble"

func _on_npc_alt_detector_body_entered(body: Node2D) -> void:
	if body.get_name() == "GarrethAlt":
		blackboard.set_value("current_state", "going back home")
		return
	
	if blackboard.get_value("current_state") == "chasing":
		return
		
	if body.get_name() == "SilasAlt":
		return
	
	if body.get_node_or_null("SeedProp"):		
		blackboard.set_value("actor", body)
			
		timer.stop()
		emote_bubble.show_emote_bubble(Globals.Emote.EXCLAMATION)
			
		blackboard.set_value("current_state", "chasing")
	pass # Replace with function body.
