extends ActionLeaf

@onready var animation_tree = $"../../../AnimationTree"
@onready var animation_state: AnimationNodeStateMachinePlayback = animation_tree.get("parameters/playback")
@onready var blackboard = $"../../../Blackboard"

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") == "hungry":
		actor.npc_active = false
		animation_state.travel("Eat")
		blackboard.set_value("current_state", "eating")
		return RUNNING
		
	if blackboard.get_value("current_state") == "done eating":
		actor.npc_active = true
		actor.handle_animation()
		
		blackboard.set_value("current_state", "idle")
		return SUCCESS
	
	return FAILURE


func _on_animation_tree_animation_finished(anim_name: StringName) -> void:
	if anim_name == "eat_left" || anim_name == "right":
		blackboard.set_value("current_state", "done eating")
	pass # Replace with function body.
