extends CharacterBody2D

@onready var navigation_agent_2d: NavigationAgent2D = $NavigationAgent2D
@onready var animation_tree: AnimationTree = $AnimationTree
@onready var animation_state: AnimationNodeStateMachinePlayback = animation_tree.get("parameters/playback")
@onready var blackboard = $Blackboard

@export var FRICTION: int = 4
@export var movement_speed: int = 60
@export var money = 100
@export var food = 5

const npc_name: String = "Silas"

var npc_active: bool = false
var current_location: Vector2

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
		handle_animation()
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
		
func move_actor(patrol_location: Vector2):
	current_location = patrol_location
	npc_active = true
	pass # Replace with function body.

func _on_navigation_agent_2d_velocity_computed(safe_velocity: Vector2) -> void:
	velocity = safe_velocity
	pass # Replace with function body.
