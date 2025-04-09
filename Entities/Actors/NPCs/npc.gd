class_name NPC
extends Actor

@export var evaluation_interval: Vector2 = Vector2(10, 20)

var lawful_trait: LawfulTrait
var thief_trait: ThiefTrait
var traits: Array[BaseTrait] = []

var is_in_knockback: bool = false

@onready var executor: Executor = $Executor
@onready var navigation_agent: NavigationAgent2D = $NavigationAgent2D
@onready var evaluation_timer: Timer = $EvaluationTimer

func _ready():
	super._ready()
	setup_traits()
	setup_resources()
	start_timer()

func setup_traits():
	traits.append($Traits/SurvivalTrait)

	lawful_trait = get_node_or_null("Traits/LawfulTrait")
	if lawful_trait != null: traits.append(lawful_trait)

	thief_trait = get_node_or_null("Traits/ThiefTrait")
	if thief_trait != null: traits.append(thief_trait)

	var farmer_trait = get_node_or_null("Traits/FarmerTrait")
	if farmer_trait != null: traits.append(farmer_trait)

func setup_resources():
	var money_resource = get_node("Resources/Money")
	resources.append(money_resource)

	var food_resource = get_node("Resources/Food")
	resources.append(food_resource)

	var satiation_resource: ResourceStat = get_node("Resources/Satiation")
	resources.append(satiation_resource)

	var companionship_resource = get_node("Resources/Companionship")
	resources.append(companionship_resource)

	var duty_resource = get_node_or_null("Resources/Duty")
	if duty_resource != null: resources.append(duty_resource)

func start_timer():
	if evaluation_timer.is_stopped():
		evaluation_timer.start(randf_range(evaluation_interval.x, evaluation_interval.y))

func _physics_process(_delta):
	if is_in_knockback:
		velocity = velocity.move_toward(Vector2.ZERO, friction)

		if velocity.length() == 0:
			is_in_knockback = false

		move_and_slide()
		return

	stop_agent()

func face_target(target):
	var direction = global_position.direction_to(target.global_position)
	set_blend_positions(direction.x)
	stop_agent()

func stop_agent():
	var new_velocity = velocity.move_toward(Vector2.ZERO, friction)
	set_agent_velocity(new_velocity)
	handle_animations()

func move_agent():
	var destination = navigation_agent.get_next_path_position()
	var direction = global_position.direction_to(destination)
	var new_velocity = velocity.move_toward(direction * max_speed, acceleration)
	set_agent_velocity(new_velocity)
	handle_animations()

func set_agent_velocity(new_velocity):
	if navigation_agent.avoidance_enabled:
		navigation_agent.velocity = new_velocity
	else:
		_on_navigation_agent_2d_velocity_computed(new_velocity)

func handle_animations():
	if velocity.length() > 0:
		if velocity.x != 0:
			set_blend_positions(velocity.x)

		animation_state.travel("Move")
	else:
		animation_state.travel("Idle")

func start_interaction(target):
	var action_data = {
		"action": Globals.Action.INTERACT,
		"data": {"target": target}
	}
	executor.start_action(action_data)

func stop_interaction():
	executor.end_action()

func crime_witnessed(crime: Crime):
	if lawful_trait != null:
		crime.investigator = self
		lawful_trait.assigned_case = crime
		
		var action_data = {
			"action": Globals.Action.PURSUIT,
			"data": {
				"case": crime,
				"target": crime.criminal,
				"assess_completed": true
			}
		}
		executor.start_action(action_data)
		return

	match crime.category:
		Crime.Category.MURDER:
			var action_data = {
				"action": Globals.Action.FLEE,
				"data": {}
			}
			executor.start_action(action_data)


func apply_knockback(direction: Vector2, force: float):
	velocity = direction * force
	animation_state.travel("Idle")
	is_in_knockback = true

func _on_evaluation_timer_timeout():
	var satiation = get_resource(Globals.ResourceType.SATIATION)

	if satiation.amount < satiation.lower_threshold:
		if override_evaluation(): return
		
	var action_data = Strategiser.evaluation_action(self, Globals.SocialPractice.PROACTIVE)

	if not action_data.is_empty():
		executor.start_action(action_data)
	else:
		print("No action evaluated by ", self.name)
		start_timer()

func override_evaluation() -> bool:
	if holds_resource(Globals.ResourceType.FOOD):
		var action_data = {"action": Globals.Action.EAT, "data": {}}
		executor.start_action(action_data)
		return true
	elif get_resource_amount(Globals.ResourceType.MONEY) > 10 \
		and WorldState.shop.food_amount > 0:
		var action_data = {"action": Globals.Action.SHOP, "data": {}}
		executor.start_action(action_data)
		return true

	return false

func _on_navigation_agent_2d_velocity_computed(safe_velocity):
	if not is_in_knockback:
		velocity = safe_velocity
		move_and_slide()

func _on_animation_tree_animation_finished(_anim_name: StringName):
	executor.procedural_tree.blackboard.set_value("anim_finished", true)

func _on_satiation_satiation_depleted():
	print(self.name, " took damage due to satiation depletion")
	apply_damage()
