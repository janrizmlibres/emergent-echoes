class_name NPC
extends Actor

enum MainState {
	WANDER = PCG.Action.WANDER,
	PETITION = PCG.Action.PETITION,
	TALK = PCG.Action.TALK,
	EAT = PCG.Action.EAT,
	SHOP = PCG.Action.SHOP,
	THEFT = PCG.Action.THEFT,
	INTERROGATE = PCG.Action.INTERROGATE,
	PURSUIT = PCG.Action.PURSUIT,
	PLANT = PCG.Action.PLANT,
	HARVEST = PCG.Action.HARVEST,
}

enum ReactState {
	NONE,
	PURSUIT = PCG.Action.PURSUIT_REACT,
	INTERACT = PCG.Action.INTERACT,
	CAUTIOUS = PCG.Action.CAUTIOUS,
	FLEE = PCG.Action.FLEE,
}

var state := State.new()
var is_in_knockback := false

@onready var executor: Executor = $Executor
@onready var navigation_agent: NavigationAgent2D = $NavigationAgent2D
@onready var radial_menu: Control = $RadialMenu

func _ready():
	super._ready()
	PCG.crime_committed.connect(on_crime_committed)
	PCG.danger_occured.connect(on_danger_occured)

func _physics_process(_delta):
	if is_in_knockback:
		handle_knockback()
	
	stop_agent()

func move_agent():
	var destination = navigation_agent.get_next_path_position()
	var direction = global_position.direction_to(destination)
	var new_velocity = velocity.move_toward(direction * max_speed, acceleration)
	set_agent_velocity(new_velocity)
	handle_animations()

func stop_agent():
	var new_velocity = velocity.move_toward(Vector2.ZERO, friction)
	set_agent_velocity(new_velocity)
	handle_animations()

func set_agent_velocity(new_velocity):
	if navigation_agent.avoidance_enabled:
		navigation_agent.velocity = new_velocity
	else:
		_on_navigation_agent_2d_velocity_computed(new_velocity)

func handle_animations():
	if velocity.length() > 0:
		if velocity.x != 0:
			set_blend_positions(velocity.x)

		animation_state.travel("Move")
	else:
		animation_state.travel("Idle")

func handle_knockback():
	velocity = velocity.move_toward(Vector2.ZERO, friction)

	if velocity == Vector2.ZERO:
		is_in_knockback = false

	move_and_slide()

func apply_knockback(direction: Vector2, force: float):
	velocity = direction * force
	animation_state.travel("Idle")
	is_in_knockback = true

func start_interaction(target: Actor):
	set_react_state(ReactState.INTERACT, {"target": target})

func stop_interaction():
	set_react_state(ReactState.NONE)

func face_target(target: Actor) -> void:
	var direction = global_position.direction_to(target.global_position)
	set_blend_positions(direction.x)
	stop_agent()

func set_main_state(new_main: MainState, data := {}):
	assert(state.react == ReactState.NONE, "Cannot set main state while in react state")
	state.main = MainData.new(new_main, data)
	run_main_state(new_main, data)

func set_react_state(new_react: ReactState, data := {}):
	PCG.stop_evaluation(self)

	if state.react == ReactState.INTERACT:
		emote_bubble.deactivate()
		WorldState.set_status(self, ActorState.State.FREE)

	state.react = new_react

	if new_react == ReactState.NONE:
		run_main_state(state.main.state, state.main.data)
		return
	elif new_react == ReactState.INTERACT:
		emote_bubble.activate()
		WorldState.set_status(self, ActorState.State.OCCUPIED)
	
	# var action := PCG.map_react_state_to_action(new_react)
	executor.start_action(new_react as int, data)

func run_main_state(main_state: MainState, data := {}):
	if main_state == MainState.WANDER:
		PCG.run_evaluation(self)
	
	# var action := PCG.map_main_state_to_action(main_state)
	executor.start_action(main_state as int, data)

# func exit_state( data := {}):

func _on_npc_agent_action_evaluated(action_data: ActionData):
	# var main_state := PCG.map_action_to_main_state(action_data.action)
	set_main_state(action_data.action as int, action_data.data)

func on_crime_committed(criminal: Actor, victim: Actor):
	if criminal == self: return
	if criminal not in actors_in_range and victim not in actors_in_range:
		return

	if not WorldState.actor_has_trait(self, "lawful"):
		executor.start_action(PCG.Action.FLEE, {"target": criminal})
		return
	
	executor.start_action(PCG.Action.PURSUIT_REACT, {"target": criminal})

func on_danger_occured(source: Actor):
	if source == self or source not in actors_in_range:
		return

	executor.pending_action = executor.current_tree.action_data
	executor.start_action(PCG.Action.CAUTIOUS, {"target": source})

func _on_navigation_agent_2d_velocity_computed(safe_velocity):
	if not is_in_knockback:
		velocity = safe_velocity
		move_and_slide()

func _on_animation_tree_animation_finished(_anim_name: StringName):
	executor.set_blackboard_value("eat_finished", true)

func actor_pressed():
	radial_menu.toggle()

class State:
	var main := MainData.new(MainState.WANDER)
	var react := ReactState.NONE

class MainData:
	var state: MainState
	var data: Dictionary

	func _init(_state: MainState, _data := {}):
		state = _state
		data = _data

# func crime_witnessed(crime: Crime):
# 	if lawful_trait != null:
# 		crime.investigator = self
# 		lawful_trait.assigned_case = crime
		
# 		var action_data = {
# 			"action": PCG.Action.PURSUIT,
# 			"data": {
# 				"case": crime,
# 				"target": crime.criminal,
# 				"assess_completed": true
# 			}
# 		}
# 		executor.start_action(action_data)
# 		return

# 	match crime.category:
# 		Crime.Category.MURDER:
# 			var action_data = {
# 				"action": PCG.Action.FLEE,
# 				"data": {}
# 			}
# 			executor.start_action(action_data)

# var satiation = WorldState.resource_manager.get_resource(self, PCG.ResourceType.SATIATION)
	# if satiation.amount < satiation.lower_threshold:
	# 	if override_evaluation(): return
# func override_evaluation() -> bool:
# 	if WorldState.resource_manager.holds_resource(self, PCG.ResourceType.FOOD):
# 		var action_data = {"action": PCG.Action.EAT, "data": {}}
# 		executor.start_action(action_data)
# 		return true
# 	elif WorldState.resource_manager.get_resource_amount(self, PCG.ResourceType.MONEY) > 10 \
# 		and WorldState.shop.food_amount > 0:
# 		var action_data = {"action": PCG.Action.SHOP, "data": {}}
# 		executor.start_action(action_data)
# 		return true
# 	return false

# func _on_satiation_satiation_depleted():
# 	print(self.name, " took damage due to satiation depletion")
# 	apply_damage()

# class State:
# 	var main := MainState.WANDER
# 	var react := ReactState.NONE
