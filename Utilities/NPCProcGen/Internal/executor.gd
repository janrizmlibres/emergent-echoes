class_name Executor
extends Node

@export var procedural_tree: BeehaveTree

func _ready():
	var blackboard = procedural_tree.blackboard
	blackboard.set_value("interaction required", false)
	blackboard.set_value("interaction_target", null)
	blackboard.set_value("action", Globals.Action.NONE)
	blackboard.set_value("action_pending", false)
	blackboard.set_value("target", null)
	blackboard.set_value("resource_type", Globals.ResourceType.NONE)
	blackboard.set_value("target_found", false)
	blackboard.set_value("target_secured", false)
	blackboard.set_value("move_position", null)

func start_evaluated_action(action: Array) -> void:
	var action_name = action[0]
	var data = action[1]

	procedural_tree.blackboard.set_value("action_pending", true)
	procedural_tree.blackboard.set_value("action", action_name)

	match action_name:
		Globals.Action.PETITION:
			procedural_tree.blackboard.set_value("target", data[0])
			procedural_tree.blackboard.set_value("resource_type", data[1])