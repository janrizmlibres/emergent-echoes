class_name PlayerAlt
extends Actor

enum State {DORMANT, ACTIVE, ATTACK}

var state: State = State.ACTIVE
var can_buy := false

@onready var remote_transform: RemoteTransform2D = $RemoteTransform2D

func _physics_process(_delta):
	match state:
		State.DORMANT:
			return
		State.ACTIVE:
			active_state()
		State.ATTACK:
			attack_state()

func _unhandled_key_input(event):
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_J:
			if can_buy:
				buy_food()
			else:
				EventManager.emit_info_dialog_requested(self)
		elif event.keycode == KEY_L:
			eat_food()

func buy_food():
	if WorldState.resource_manager.get_resource(self, PCG.ResourceType.MONEY).amount < 10:
		return

	var resource_mgr := WorldState.resource_manager
	resource_mgr.modify_resource(self, PCG.ResourceType.FOOD, 1)
	resource_mgr.modify_resource(self, PCG.ResourceType.MONEY, -10)
	float_text_controller.show_float_text(
		PCG.ResourceType.FOOD,
		str(1),
		true
	)

func eat_food() -> void:
	if WorldState.resource_manager.get_resource(self, PCG.ResourceType.FOOD).amount <= 0:
		return

	var resource_mgr := WorldState.resource_manager
	resource_mgr.modify_resource(self, PCG.ResourceType.FOOD, -1)
	resource_mgr.modify_resource(self, PCG.ResourceType.SATIATION, 30)
	float_text_controller.show_float_text(
		PCG.ResourceType.SATIATION,
		str(30),
		true
	)

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

func apply_knockback(direction: Vector2, force: float):
	velocity = direction * force

func do_handle_detainment(detainer: Actor):
	remove_child(remote_transform)
	detainer.add_child(remote_transform)

func do_handle_captivity(detainer: Actor):
	detainer.remove_child(remote_transform)
	add_child(remote_transform)

func _on_animation_tree_animation_finished(anim_name: StringName):
	if anim_name.contains("attack"):
		state = State.ACTIVE

func _on_radius_actionable_body_entered(body: Node2D):
	if body == self or body is not NPC:
		return
	
	body.radial_menu.enable_petition()

func _on_radius_actionable_body_exited(body: Node2D):
	if body is not NPC:
		return
	
	body.radial_menu.disable_petition()

func _on_player_shop_body_entered(body: Node2D):
	if body == self:
		can_buy = true

func _on_player_shop_body_exited(body: Node2D):
	if body == self:
		can_buy = false

func start_interaction(target):
	var direction = global_position.direction_to(target.global_position)
	animation_tree.set("parameters/Idle/blend_position", direction.x)
	state = State.DORMANT

func stop_interaction():
	state = State.ACTIVE