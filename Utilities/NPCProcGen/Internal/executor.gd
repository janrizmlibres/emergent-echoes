class_name Executor
extends Node

var starting_bt: PackedScene = preload("res://Entities/Actors/NPCs/AI/Trees/wander_bt.tscn")

var pending_action: ActionData
var current_action: BTData

func _ready():
	instantiate_bt(starting_bt, PCG.Action.NONE)

func start_action(action: PCG.Action, data: Dictionary) -> void:
	var action_string := PCG.action_to_string(action).to_lower()
	var bt_scene: PackedScene = load(
		"res://Entities/Actors/NPCs/AI/Trees/" + action_string + "_bt.tscn"
	)

	var bt := instantiate_bt(bt_scene, action, data)
	bt.blackboard.set_value("data", data)
	WorldState.set_current_action(bt.actor, action)
	PCG.stop_evaluation(bt.actor)

func end_action() -> void:
	current_action.tree.queue_free()

	if pending_action == null:
		var bt := instantiate_bt(starting_bt, PCG.Action.NONE)
		PCG.run_evaluation(bt.actor)
	else:
		start_action(pending_action.action, pending_action.data)
		pending_action = null

func instantiate_bt(bt_scene: PackedScene, action: PCG.Action, data := {}) -> BeehaveTree:
	var bt: BeehaveTree = bt_scene.instantiate()
	bt.actor_node_path = get_parent().get_path()
	current_action = BTData.new(ActionData.new(action, data), bt)
	add_child.call_deferred(bt)
	return bt

func set_blackboard_value(key: Variant, value: Variant) -> void:
	assert(current_action != null, "current_action is null")
	current_action.tree.blackboard.set_value(key, value)

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

class BTData:
	var action_data: ActionData
	var tree: BeehaveTree

	func _init(_action_data: ActionData, _tree: BeehaveTree):
		action_data = _action_data
		tree = _tree