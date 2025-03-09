class_name NPC
extends Actor

@export var evaluation_interval: Vector2 = Vector2(10, 20)

var lawful_trait: LawfulTrait
var thief_trait: ThiefTrait
var traits: Array[BaseTrait] = []

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

	var satiation_resource = get_node("Resources/Satiation")
	resources.append(satiation_resource)

	var companionship_resource = get_node("Resources/Companionship")
	resources.append(companionship_resource)

func start_timer():
	if evaluation_timer.is_stopped():
		evaluation_timer.start(randf_range(evaluation_interval.x, evaluation_interval.y))

func _physics_process(_delta):
	stop_agent()

func face_target(target):
	var direction = global_position.direction_to(target.global_position)
	animation_tree.set("parameters/Idle/blend_position", direction.x)
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
	executor.procedural_tree.blackboard.set_value("interaction_target", target)
	executor.procedural_tree.blackboard.set_value("interaction_required", true)
	evaluation_timer.stop()

func stop_interaction():
	executor.procedural_tree.blackboard.set_value("interaction_required", false)
	start_timer()

func _on_evaluation_timer_timeout():
	var action: Array = Strategiser.evaluation_action(self, Globals.SocialPractice.PROACTIVE)

	if not action.is_empty():
		print("Action evaluated by ", self.name, ": ", Globals.get_action_enum_string(action[0]))
		executor.start_evaluated_action(action)
	else:
		start_timer()

func _on_navigation_agent_2d_velocity_computed(safe_velocity):
	velocity = safe_velocity
	move_and_slide()

func _on_animation_tree_animation_finished(anim_name: StringName):
	if anim_name.contains("eat"):
		executor.procedural_tree.blackboard.set_value("eat_finished", true)
