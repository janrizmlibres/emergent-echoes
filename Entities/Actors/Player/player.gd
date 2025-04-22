class_name Player
extends Actor

enum State {DORMANT, ACTIVE, ATTACK}

var state: State = State.ACTIVE

@onready var remote_transform: RemoteTransform2D = $RemoteTransform2D

func _physics_process(_delta):
	match state:
		State.DORMANT:
			return
		State.ACTIVE:
			active_state()
		State.ATTACK:
			attack_state()

func active_state():
	var input_vector = Input.get_vector("left", "right", "up", "down")
	move(input_vector)

	if Input.is_action_just_pressed("use"):
		if Input.is_mouse_button_pressed(MOUSE_BUTTON_LEFT):
			var click_direction := global_position.direction_to(get_global_mouse_position())
			set_blend_positions(click_direction.x)

		state = State.ATTACK
	
func attack_state():
	velocity = Vector2.ZERO
	animation_state.travel("Attack")

func idle():
	velocity = velocity.move_toward(Vector2.ZERO, friction)
	animation_state.travel("Idle")

func move(direction_vector: Vector2):
	if direction_vector.length() > 0:
		if direction_vector.x != 0:
			set_blend_positions(direction_vector.x)

		velocity = velocity.move_toward(direction_vector * max_speed, acceleration)
		animation_state.travel("Move")
	else:
		idle()

	move_and_slide()

func start_interaction(target):
	var direction = global_position.direction_to(target.global_position)
	set_blend_positions(direction.x)
	state = State.DORMANT

func stop_interaction():
	state = State.ACTIVE

func apply_knockback(direction: Vector2, force: float):
	velocity = direction * force

func _on_animation_tree_animation_finished(anim_name: StringName):
	if anim_name.contains("attack"):
		state = State.ACTIVE