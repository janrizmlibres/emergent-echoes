class_name PlayerAlt
extends Actor

enum State {DORMANT, ACTIVE, ATTACK}

var state: State = State.ACTIVE
var can_buy := false

var satiation := 1.0:
	get:
		return satiation
	set(value):
		satiation = clamp(value, 0, 100)

var money := 20
var food := 2

func _physics_process(_delta):
	match state:
		State.DORMANT:
			return
		State.ACTIVE:
			active_state()
		State.ATTACK:
			attack_state()

func _process(delta):
	satiation -= delta * 0.2

	if satiation <= 0 and hunger_dmg_cooldown <= 0:
		hunger_dmg_cooldown = HUNGER_DMG_INTERVAL
		apply_damage()

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
	if money < 10:
		return

	food += 1
	money -= 10

	float_text_controller.show_float_text(
		PCG.ResourceType.FOOD,
		str(1),
		true
	)

func eat_food() -> void:
	if food <= 0:
		return

	food -= 1
	satiation += 30
	
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

func idle_state():
	velocity = velocity.move_toward(Vector2.ZERO, friction)
	animation_state.travel("Idle")

func move(direction_vector: Vector2):
	if direction_vector.length() > 0:
		if direction_vector.x != 0:
			set_blend_positions(direction_vector.x)

		velocity = velocity.move_toward(direction_vector * max_speed, acceleration)
		animation_state.travel("Move")
	else:
		idle_state()

	move_and_slide()

func apply_knockback(direction: Vector2, force: float):
	velocity = direction * force

func do_handle_detainment(detainer: Actor):
	%Camera2D.current_actor = detainer

func do_handle_release():
	%Camera2D.current_actor = self

func _on_animation_tree_animation_finished(anim_name: StringName):
	if anim_name.contains("attack"):
		state = State.ACTIVE

func _on_radius_actionable_body_entered(body: Node2D):
	if body != self and body.has_node_and_resource("RadialMenu"):
		body.radial_menu.enable_petition()

func _on_radius_actionable_body_exited(body: Node2D):
	if body.has_node_and_resource("RadialMenu"):
		body.radial_menu.disable_petition()

func _on_player_shop_body_entered(body: Node2D):
	if body == self:
		can_buy = true

func _on_player_shop_body_exited(body: Node2D):
	if body == self:
		can_buy = false

func start_interaction(target, _action, _resource_type):
	var direction = global_position.direction_to(target.global_position)
	animation_tree.set("parameters/Idle/blend_position", direction.x)
	state = State.DORMANT

func stop_interaction():
	state = State.ACTIVE
