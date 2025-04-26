extends BaseCamera

@export var floating_speed: int = 100
var is_attached: bool = true

@onready var current_actor: Actor = %Player:
	get:
		return current_actor
	set(value):
		current_actor = value

		if current_actor is Player and not is_attached:
			current_actor.state = Player.State.DORMANT

func _physics_process(delta):
	if is_attached and current_actor != null:
		global_position = current_actor.global_position
	else:
		move(delta)

	if Input.is_action_just_pressed("toggle_camera"):
		toggle_camera()

func move(delta):
	var input_vector = Input.get_vector("left", "right", "up", "down")
	global_position += input_vector * floating_speed * delta

func toggle_camera():
	# var player = get_node_or_null("%Player")
	# var is_player_alive = player != null
	if current_actor == null:
		current_actor = get_node_or_null("%Player")
		is_attached = current_actor != null

		if not is_attached:
			return
	else:
		is_attached = !is_attached
		# if current_actor is NPC and not is_player_alive:
		# 	is_attached = false
		# else:
		# 	is_attached = !is_attached

	if current_actor is NPC:
		return

	current_actor.state = Player.State.ACTIVE if is_attached else Player.State.DORMANT

func find_actor():
	pass
