class_name Executor
extends Node

var current_tree: BeehaveTree

func _ready():
	current_tree = get_child(0)

func start_action(action: PCG.Action, data := {}) -> void:
	var action_string := PCG.action_to_string(action).to_lower()
	var bt_scene: PackedScene = load(
		"res://Entities/Actors/NPCs/AI/Trees/" + action_string + "_bt.tscn"
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
	assert(current_tree != null, "current_tree is null")
	current_tree.blackboard.set_value(key, value)

# class BTData:
# 	var action_data: ActionData
# 	var tree: BeehaveTree

# 	func _init(_action_data: ActionData, _tree: BeehaveTree):
# 		action_data = _action_data
# 		tree = _tree

# func end_action() -> void:
# 	current_tree.tree.queue_free()

# 	if pending_action == null:
# 		var bt := instantiate_bt(starting_bt, PCG.Action.WANDER)
# 		PCG.run_evaluation(bt.actor)
# 	else:
# 		start_action(pending_action.action, pending_action.data)
	
# 	pending_action = null

# func setup_data(action: PCG.Action):
# 	var actor: Actor = bt.actor as Actor
# 	if action == PCG.Action.PLANT:
# 		actor.seed_prop.visible = true
# 	if [PCG.Action.PURSUIT, PCG.Action.PLANT, PCG.Action.HARVEST,
# 		PCG.Action.FLEE].has(action):
# 		WorldState.set_availability(actor, false)
# func end_action():
# 	var npc = bt.actor as NPC
# 	WorldState.set_current_action(npc, PCG.Action.NONE)
# 	bt.blackboard.set_value("action", PCG.Action.NONE)
# 	bt.blackboard.erase_value("data")
# 	npc.start_timer()
# func reset_data():
# 	var actor = bt.actor as Actor
# 	actor.seed_prop.visible = false
# 	WorldState.set_is_busy(actor, false)
# 	WorldState.set_availability(actor, true)
# 	var blackboard = bt.blackboard
# 	blackboard.set_value("target_reached", false)
# 	blackboard.set_value("anim_finished", false)
# func set_enable(value: bool):
# 	bt.enabled = value
