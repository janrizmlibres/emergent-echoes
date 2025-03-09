class_name Player
extends Actor

func _ready():
	super._ready()
	setup_resources()

func setup_resources():
	var money_resource = get_node("Resources/Money")
	resources.append(money_resource)

	var food_resource = get_node("Resources/Food")
	resources.append(food_resource)

func _physics_process(_delta):
	var input_vector = Input.get_vector("left", "right", "up", "down")

	if input_vector.length() > 0:
		if input_vector.x != 0:
			set_blend_positions(input_vector.x)

		velocity = velocity.move_toward(input_vector * max_speed, acceleration)
		animation_state.travel("Move")
	else:
		velocity = velocity.move_toward(Vector2.ZERO, friction)
		animation_state.travel("Idle")

	move_and_slide()