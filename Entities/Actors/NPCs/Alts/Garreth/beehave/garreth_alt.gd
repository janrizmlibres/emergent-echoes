extends CharacterBody2D

@onready var float_text_controller = $FloatTextController
@onready var animation_tree: AnimationTree = $AnimationTree
@onready var animation_state: AnimationNodeStateMachinePlayback = animation_tree.get("parameters/playback")
@onready var navigation_agent_2d: NavigationAgent2D = $NavigationAgent2D
@onready var blackboard = $Blackboard
@onready var radial_menu: RadialMenu = $RadialMenu

@export var FRICTION: int = 4
@export var movement_speed: int = 60
@export var money = 100
@export var food = 5

const npc_name: String = "Garreth"

var npc_active: bool = false
var current_location: Vector2

var player_reached = false
var silas_reached = false

func _physics_process(_delta: float) -> void:
	if npc_active and current_location:
		navigation_agent_2d.target_position = current_location
		
		if navigation_agent_2d.is_navigation_finished():
			blackboard.set_value("agent_arrived", true)
			npc_active = false
			velocity = velocity.move_toward(Vector2.ZERO, FRICTION)
		else:
			var current_agent_position: Vector2 = global_position
			var next_path_position: Vector2 = navigation_agent_2d.get_next_path_position()
			var new_velocity: Vector2 = current_agent_position.direction_to(next_path_position) * movement_speed
			
			if navigation_agent_2d.avoidance_enabled:
				navigation_agent_2d.set_velocity(new_velocity)
			else:
				_on_navigation_agent_2d_velocity_computed(new_velocity)
			
		handle_animation()
		move_and_slide()
	else:
		velocity = velocity.move_toward(Vector2.ZERO, FRICTION)

func handle_animation() -> void:
	if velocity.x != 0:
		animation_tree.set("parameters/Idle/blend_position", velocity.x)
		animation_tree.set("parameters/Move/blend_position", velocity.x)
		animation_state.travel("Move")
	elif velocity.y != 0:
		animation_state.travel("Move")
	else:
		animation_state.travel("Idle")
		
func face_target(target):
	var direction = global_position.direction_to(target.global_position)
	animation_tree.set("parameters/Idle/blend_position", direction.x)

func move_actor(patrol_location: Vector2):
	current_location = patrol_location
	npc_active = true
	pass # Replace with function body.
	
func set_animation_to_idle():
	animation_state.travel("Idle")

func _on_navigation_agent_2d_velocity_computed(safe_velocity: Vector2) -> void:
	velocity = safe_velocity
	pass # Replace with function body.
	
func _on_hurt_box_area_entered(area: Area2D) -> void:
	if area.get_name() == "Weapon":
		queue_free()
		return
	pass # Replace with function body.

func _on_silas_detector_body_entered(body: Node2D) -> void:
	if body.get_name() == "ToblinAlt":
		return
	
	if blackboard.get_value("cutscene_state") == "go to silas" && body.get_name() == "SilasAlt":
		silas_reached = true
		return
	
	if blackboard.get_value("cutscene_state") == "go to player" && body.get_name() == "Player":
		player_reached = true
		return
	pass # Replace with function body.

func _on_hover_area_input_event(_viewport: Node, event: InputEvent, _shape_idx: int):
	if event is InputEventMouseButton:
		if event.is_released() and event.button_index == MOUSE_BUTTON_RIGHT:
			actor_pressed()

func actor_pressed():
	radial_menu.toggle()
