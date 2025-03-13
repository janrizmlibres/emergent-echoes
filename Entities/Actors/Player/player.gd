class_name Player
extends Actor

enum State {IDLE, MOVING, ATTACK}

var state: State = State.MOVING

@onready var remote_transform: RemoteTransform2D = $RemoteTransform2D

func _ready():
	super._ready()
	setup_resources()

func setup_resources():
	var money_resource = get_node("Resources/Money")
	resources.append(money_resource)

	var food_resource = get_node("Resources/Food")
	resources.append(food_resource)

func _physics_process(_delta):
	match state:
		State.IDLE:
			idle()
		State.MOVING:
			move_state()
		State.ATTACK:
			velocity = Vector2.ZERO
			animation_state.travel("Attack")

	if Input.is_action_just_pressed("attack"):
		state = State.ATTACK

func move_state():
	var input_vector = Input.get_vector("left", "right", "up", "down")

	if input_vector.length() > 0:
		if input_vector.x != 0:
			set_blend_positions(input_vector.x)

		velocity = velocity.move_toward(input_vector * max_speed, acceleration)
		animation_state.travel("Move")
	else:
		idle()

	move_and_slide()

func idle():
	velocity = velocity.move_toward(Vector2.ZERO, friction)
	animation_state.travel("Idle")

func start_interaction(target):
	var direction = global_position.direction_to(target.global_position)
	animation_tree.set("parameters/Idle/blend_position", direction.x)
	state = State.IDLE

func stop_interaction():
	state = State.MOVING

func apply_knockback(direction: Vector2, force: float):
	velocity = direction * force

func _on_animation_tree_animation_finished(anim_name: StringName):
	if anim_name.contains("attack"):
		state = State.MOVING