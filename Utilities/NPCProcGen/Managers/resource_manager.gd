class_name ResourceManager
extends Node

var total_food := TotalFoodResource.new()
var _actor_resources: Dictionary[Actor, Array] = {}

func _ready():
	total_food.name = "TotalFood"
	add_child(total_food)

func register_actor(actor: Actor, agent: PCGAgent):
	assert(actor not in _actor_resources, "ResourceManager: Actor already registered")
	
	var actor_container = Node.new()
	actor_container.name = actor.name
	add_child(actor_container)

	_actor_resources[actor] = []

	var money_resource := MoneyResource.new(agent.money_final_amount, agent.money_final_weight)
	store_resource(actor, money_resource, "Money", actor_container)

	var food_resource := FoodResource.new(agent.food_final_amount, agent.food_final_weight)
	store_resource(actor, food_resource, "Food", actor_container)

	var satiation_resource := SatiationResource.new(
		actor,
		agent.satiation_final_amount,
		agent.satiation_final_weight
	)
	store_resource(actor, satiation_resource, "Satiation", actor_container)
	
	if agent is NPCAgent:
		var companionship_resource := CompanionshipResource.new(
			agent.companionship_amount,
			agent.companionship_weight
		)
		store_resource(actor, companionship_resource, "Companionship", actor_container)

		if WorldState.npc_manager.has_trait(actor, "lawful"):
			var duty_resource := DutyResource.new(agent.duty_amount, agent.duty_weight)
			store_resource(actor, duty_resource, "Duty", actor_container)
	
	update_total_food(actor)

func unregister_actor(actor: Actor):
	get_node(NodePath(actor.name)).queue_free()
	_actor_resources.erase(actor)

func store_resource(actor: Actor, resource: BaseResource, node_name: String, container: Node):
	_actor_resources[actor].append(resource)
	resource.name = node_name
	container.add_child(resource)

func update_total_food(actor: Actor):
	total_food.amount += get_resource_amount(actor, PCG.ResourceType.FOOD)

	var actor_count := WorldState.get_actor_count()
	total_food.set_thresholds(
		PCG.food_lower_threshold * actor_count,
		PCG.food_upper_threshold * actor_count
	)
	
func holds_resource(actor: Actor, type: PCG.ResourceType) -> bool:
	var resource = get_resource(actor, type)
	return resource.amount > 0 if resource != null else false

func resource_deficient(actor: Actor, type: PCG.ResourceType) -> bool:
	var resource = get_resource(actor, type)
	assert(resource != null, "Actor must have resource")
	return resource.amount < resource.lower_threshold

func get_resource(actor: Actor, type: PCG.ResourceType) -> BaseResource:
	var resources := _actor_resources[actor]
	var idx := resources.find_custom(func(res: BaseResource): return res.type == type)
	return resources[idx] if idx != -1 else null

func get_resource_amount(actor: Actor, type: PCG.ResourceType) -> float:
	var resource = get_resource(actor, type)
	return resource.amount if resource != null else 0

func modify_resource(actor: Actor, type: PCG.ResourceType, amount: float):
	var resource = get_resource(actor, type)
	if resource == null: return
	resource.amount += amount

func transfer_resource(
	sender: Actor,
	receiver: Actor,
	resource_type: PCG.ResourceType,
	amount: float
):
	var sender_resource := get_resource(sender, resource_type)
	var receiver_resource := get_resource(receiver, resource_type)
	sender_resource.amount -= amount
	receiver_resource.amount += amount
