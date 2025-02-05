extends ConditionLeaf

@onready var chase_player_leaf = $"../../Chase Player"

var player_near: bool = false

func tick(actor: Node, blackboard: Blackboard) -> int:
	if player_near:
		print("Attempting to stop chase player action leaf")
		chase_player_leaf.interrupt(actor, blackboard)
		return SUCCESS
	else:
		return FAILURE

func _on_steal_area_body_entered(body: Node2D) -> void:
	if body.get_node_or_null("ActorTag2D"):
		print("Silas is on reach with the player and is about to steal.")
		player_near = true
	pass # Replace with function body.
