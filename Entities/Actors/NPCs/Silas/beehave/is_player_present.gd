extends ConditionLeaf

@onready var blackboard_object = $"../../../../Blackboard"

var player

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("player_found") == true:
		blackboard.set_value("player", player)
		blackboard.set_value("is_idle", false)
		return SUCCESS
		
	return FAILURE

func _on_npc_alt_detector_body_entered(body: Node2D) -> void:
	if body.get_node_or_null("ActorTag2D"):
		player = body
		blackboard_object.set_value("player_found", true)
	pass # Replace with function body.
