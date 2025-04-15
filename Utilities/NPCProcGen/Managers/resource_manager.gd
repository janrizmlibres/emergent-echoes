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

	store_resource(actor, MoneyResource.new(agent.money_final), actor_container)
	store_resource(actor, FoodResource.new(agent.food_final), actor_container)
	
	if agent is NPCAgent:
		store_resource(actor, SatiationResource.new(agent.money_amount), actor_container)
		store_resource(actor, CompanionshipResource.new(agent.duty_amount), actor_container)

		if WorldState.npc_manager.is_lawful(actor):
			store_resource(actor, DutyResource.new(agent.duty_amount), actor_container)
	
	update_total_food(actor)

func update_total_food(actor: Actor):
	total_food.amount += get_resource_amount(actor, PCG.ResourceType.FOOD)

	var actor_count := WorldState.get_actor_count()
	total_food.lower_threshold = PCG.food_lower_threshold * actor_count
	total_food.upper_threshold = PCG.food_upper_threshold * actor_count

func store_resource(actor: Actor, resource: BaseResource, container: Node):
	_actor_resources[actor].append(resource)
	container.add_child(resource)
	
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