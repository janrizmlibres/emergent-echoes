class_name Executor
extends Node

@export var procedural_tree: BeehaveTree

var action_stack: Array = []

func _ready():
	var blackboard = procedural_tree.blackboard

	blackboard.set_value("action", Globals.Action.NONE)
	blackboard.set_value("data", null)
	blackboard.set_value("move_position", null)

	blackboard.set_value("target", null)
	blackboard.set_value("target_reached", false)
	blackboard.set_value("anim_finished", false)

	blackboard.set_value("prison_marker", null)
	blackboard.set_value("crop_tile", null)
	blackboard.set_value("shop", null)

func start_action(action_data: Dictionary) -> void:
	var blackboard = procedural_tree.blackboard
	blackboard.set_value("action", action_data.action)
	blackboard.set_value("target", action_data.data.get("target"))
	blackboard.set_value("data", action_data.data)

	var last_action = action_stack.back() if not action_stack.is_empty() else null
	if last_action != null and last_action.action == Globals.Action.INTERACT:
		action_stack.pop_back()

	setup_data(action_data.action)
	
	action_stack.append(action_data)
	Logger.info(procedural_tree.actor.name + " started action: "
		+ Globals.get_action_string(action_data.action))

	var stack_str = "Action stack of " + procedural_tree.actor.name + ": "
	for action in action_stack:
		stack_str += Globals.get_action_string(action.action) + " -> "
	print(stack_str)

	var npc = procedural_tree.actor as NPC
	WorldState.set_current_action(npc, action_data.action)
	npc.evaluation_timer.stop()

func setup_data(action: Globals.Action):
	var actor: Actor = procedural_tree.actor as Actor

	if action == Globals.Action.PLANT:
		actor.seed_prop.visible = true
	
	if [Globals.Action.PURSUIT, Globals.Action.PLANT, Globals.Action.HARVEST,
		Globals.Action.FLEE].has(action):
		WorldState.set_availability(actor, false)

func end_action():
	if action_stack.is_empty(): return

	var tree_actor = procedural_tree.actor
	if not is_instance_valid(tree_actor) or tree_actor.is_queued_for_deletion():
		return

	var completed_action = action_stack.pop_back()
	assert(completed_action != null, "No action to end")
	Logger.info(procedural_tree.actor.name + " ended action: "
		+ Globals.get_action_string(completed_action.action))
	reset_data()

	if not action_stack.is_empty():
		var action_data = action_stack.pop_back()
		start_action(action_data)
		return
	
	var npc = procedural_tree.actor as NPC
	WorldState.set_current_action(npc, Globals.Action.NONE)
	procedural_tree.blackboard.set_value("action", Globals.Action.NONE)
	procedural_tree.blackboard.erase_value("data")
	npc.start_timer()

func reset_data():
	var actor = procedural_tree.actor as Actor
	actor.seed_prop.visible = false
	WorldState.set_is_busy(actor, false)
	WorldState.set_availability(actor, true)
	
	var blackboard = procedural_tree.blackboard
	blackboard.set_value("target_reached", false)
	blackboard.set_value("anim_finished", false)

func set_enable(value: bool):
	procedural_tree.enabled = value
