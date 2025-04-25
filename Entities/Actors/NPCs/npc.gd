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
	ASSESS = PCG.Action.ASSESS,
	PLANT = PCG.Action.PLANT,
	HARVEST = PCG.Action.HARVEST,
}

enum ReactState {
	NONE,
	PURSUIT = PCG.Action.PURSUIT,
	FLEE = PCG.Action.FLEE,
	CAUTIOUS = PCG.Action.CAUTIOUS,
	INTERACT = PCG.Action.INTERACT,
}

var state := State.new()
var is_in_knockback := false

@onready var executor: Executor = $Executor
@onready var navigation_agent: NavigationAgent2D = $NavigationAgent2D
@onready var radial_menu: RadialMenu = $RadialMenu

func _ready():
	super._ready()
	PCG.threat_present.connect(_on_threat_present)
	PCG.duty_conducted.connect(on_duty_conducted)

func _physics_process(_delta):
	if is_in_knockback:
		handle_knockback()
	
	stop_agent()

func move_agent():
	var destination = navigation_agent.get_next_path_position()
	var direction = global_position.direction_to(destination)
	var new_velocity = velocity.move_toward(direction * max_speed, acceleration)
	set_agent_velocity(new_velocity)

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

func set_main_state(new_main: MainState, data := {}):
	assert(state.react == ReactState.NONE, "Cannot set main state while in react state")

	reset_variables()

	if state.main.state == MainState.PLANT:
		var crop_tile: CropTile = executor.get_blackboard_value("crop_tile")

		if crop_tile != null:
			crop_tile.is_attended = false

	state.main = MainData.new(new_main, data)
	run_main_state(new_main, data)

func set_react_state(new_react: ReactState, data := {}):
	if state.react != ReactState.NONE and state.react < new_react:
		return

	PCG.stop_evaluation(self)
	reset_variables()

	state.react = new_react

	if new_react == ReactState.NONE:
		run_main_state(state.main.state, state.main.data)
		return
	
	if new_react == ReactState.PURSUIT:
		WorldState.set_status(self, ActorState.State.INDISPOSED)
	elif new_react == ReactState.INTERACT:
		emote_bubble.activate()
		WorldState.set_status(self, ActorState.State.OCCUPIED)
	
	executor.start_action(new_react as int, data)

func run_main_state(main_state: MainState, data := {}):
	if main_state == MainState.WANDER:
		if WorldState.actor_in_status(self, ActorState.State.FREE):
			PCG.run_evaluation(self)
	elif main_state == MainState.EAT:
		WorldState.set_status(self, ActorState.State.OCCUPIED)
	elif main_state == MainState.PURSUIT:
		WorldState.set_status(self, ActorState.State.INDISPOSED)
	elif main_state == MainState.PLANT:
		seed_prop.show()
	
	executor.start_action(main_state as int, data)

func reset_variables():
	WorldState.set_status(self, ActorState.State.FREE)
	emote_bubble.deactivate()
	seed_prop.hide()

func handle_assault(target: Actor):
	if not WorldState.npc_manager.has_trait(self, "lawful"):
		set_react_state(NPC.ReactState.FLEE, {"target": target})
		return
	
	if WorldState.is_interceptable(target):
		set_react_state(NPC.ReactState.PURSUIT, {"target": target, "is_reactive": true})

func handle_crime_committed(crime: Crime):
	handle_assault(crime.criminal)

func do_handle_detainment(_detainer: Actor):
	PCG.stop_evaluation(self)
	executor.current_tree.disable()

func do_handle_captivity(_detainer: Actor):
	executor.current_tree.enable()

func actor_pressed():
	radial_menu.toggle()

func _on_threat_present(source: Actor, recipient: Actor):
	if source == self or recipient == self or source not in actors_in_range:
		return

	set_react_state(NPC.ReactState.CAUTIOUS, {"target": source})

func on_duty_conducted(actor: Actor, is_success: bool):
	if actor != self:
		return
	
	float_text_controller.show_float_text(
		PCG.ResourceType.DUTY,
		str(30 if is_success else -1),
		true
	)

func _on_npc_agent_action_evaluated(action_data: ActionData):
	set_main_state(action_data.action as int, action_data.data)

func _on_navigation_agent_2d_velocity_computed(safe_velocity):
	if not is_in_knockback:
		velocity = safe_velocity
		move_and_slide()

func _on_animation_tree_animation_finished(anim_name: StringName):
	if anim_name.contains("eat"):
		executor.set_blackboard_value("eat_finished", true)
	elif anim_name.contains("harvest"):
		executor.set_blackboard_value("harvest_finished", true)

class State:
	var main := MainData.new(MainState.WANDER)
	var react := ReactState.NONE

class MainData:
	var state: MainState
	var data: Dictionary

	func _init(_state: MainState, _data := {}):
		state = _state
		data = _data

# func _on_satiation_satiation_depleted():
# 	print(self.name, " took damage due to satiation depletion")
# 	apply_damage()
