class_name Executor
extends Node

var current_tree: BeehaveTree

func _ready():
	current_tree = get_child(0)

func start_action(action: PCG.Action, data := {}) -> void:
	var action_string := PCG.action_to_string(action).to_lower()
	var bt_scene: PackedScene = load(
		"res://Utilities/AI/Trees/" + action_string + "_bt.tscn"
	)
	
	current_tree.queue_free()
	current_tree = bt_scene.instantiate()
	current_tree.actor = get_parent()
	add_child(current_tree)
	current_tree.blackboard.set_value("data", data)

	WorldState.set_current_action(current_tree.actor, action)

	print(current_tree.actor.name, " started action: ", action_string)

func instantiate_bt(bt_scene: PackedScene) -> BeehaveTree:
	current_tree.queue_free()
	current_tree = bt_scene.instantiate()
	current_tree.actor = get_parent()
	add_child(current_tree)
	return current_tree

func set_blackboard_value(key: Variant, value: Variant) -> void:
	current_tree.blackboard.set_value(key, value)

func get_blackboard_value(key: Variant) -> Variant:
	return current_tree.blackboard.get_value(key)
