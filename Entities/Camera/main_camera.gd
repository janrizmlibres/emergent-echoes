extends BaseCamera

@export var floating_speed: int = 100
var is_attached: bool = true

var player: Player

func _ready():
	super._ready()
	player = get_tree().root.get_node("World/YSort/Player")

func _physics_process(delta):
	if is_attached:
		global_position = player.global_position
	else:
		move(delta)

	if Input.is_action_just_pressed("toggle_camera"):
		toggle_camera()

func move(delta):
	var input_vector = Input.get_vector("left", "right", "up", "down")
	global_position += input_vector * floating_speed * delta

func toggle_camera():
	is_attached = !is_attached

	if is_attached:
		position_smoothing_enabled = true
		player.state = Player.State.ACTIVE
	else:
		position_smoothing_enabled = false
		player.state = Player.State.DORMANT