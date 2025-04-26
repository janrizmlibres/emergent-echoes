class_name Player
extends Actor

enum State {DORMANT, ACTIVE, ATTACK}

var state: State = State.ACTIVE:
	get:
		return state
	set(value):
		state = value
		var is_dormant := state == State.DORMANT
		set_process_unhandled_input(!is_dormant)
		set_process_unhandled_key_input(!is_dormant)

var can_buy := false

func _physics_process(_delta):
	match state:
		State.DORMANT:
			idle()
		State.ACTIVE:
			active_state()
		State.ATTACK:
			attack_state()

func _unhandled_input(event):
	if event.is_action_pressed("use"):
		if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT:
			var click_direction := global_position.direction_to(get_global_mouse_position())
			set_blend_positions(click_direction.x)
		
		state = State.ATTACK

func _unhandled_key_input(event):
	if event.pressed:
		if event.keycode == KEY_J:
			if can_buy:
				buy_food()
			else:
				eat_food()
		elif event.keycode == KEY_L:
			EventManager.emit_info_dialog_requested(self)

func buy_food():
	if WorldState.resource_manager.get_resource(self, PCG.ResourceType.MONEY).amount < 10:
		return
	
	if WorldState.shop.food_amount <= 0:
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
	state = State.DORMANT
	%Camera2D.current_actor = detainer

func do_handle_captivity():
	state = State.ACTIVE
	%Camera2D.current_actor = self

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
