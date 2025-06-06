class_name BaseTrait
extends Node

var npc: NPC
var weight: float

var action_candidates: Array[ActionCandidate] = []

func _init(_npc: NPC, _weight: float):
	npc = _npc
	weight = _weight

func evaluation_action(social_practice: PCG.SocialPractice) -> Array[ActionCandidate]:
	action_candidates.clear()

	match social_practice:
		PCG.SocialPractice.PROACTIVE:
			evaluation_proactive_action()

	return action_candidates.duplicate()

func evaluation_proactive_action():
	pass

func get_actor_candidates(
	resource_type: PCG.ResourceType,
	filter: Callable = Callable()
) -> Array[Actor]:
	var candidates: Array[Actor] = []
	var peer_actors: Array[Actor] = WorldState.get_peer_actors(npc).duplicate()
	peer_actors.shuffle()

	if not filter.is_null():
		peer_actors = peer_actors.filter(filter)
	
	for actor in peer_actors:
		if actor is Player and randf() > 0.2: continue
		if not WorldState.resource_manager.holds_resource(actor, resource_type):
			continue

		if WorldState.is_interactable(actor):
			candidates.append(actor)
	
	return candidates

func add_action(action: PCG.Action, resource_type: PCG.ResourceType, data = {}) -> void:
	if not validate_action(action):
		return

	var action_weight := calculate_weight(resource_type)
	var action_data := ActionData.new(action, data)
	action_candidates.append(ActionCandidate.new(action_data, action_weight))

func add_targetted_action(
	action: PCG.Action,
	resource_type: PCG.ResourceType,
	actor_selector: Callable,
	filter := Callable()
) -> void:
	var actor_candidates = get_actor_candidates(resource_type, filter)
	var target_actor = actor_selector.call(actor_candidates)
	if (target_actor == null): return

	add_action(action, resource_type, {
		"target": target_actor,
		"resource_type": resource_type
	})

func validate_action(action: PCG.Action) -> bool:
	match action:
		PCG.Action.SHOP:
			var amount := WorldState.resource_manager.get_resource_amount(
				npc, PCG.ResourceType.MONEY
			)

			if not amount > 10:
				return false

			return WorldState.shop.food_amount > 0
		PCG.Action.PLANT:
			return WorldState.some_crop_in_status(CropTile.Status.DORMANT)
		PCG.Action.HARVEST:
			return WorldState.some_crop_in_status(CropTile.Status.MATURE)
		PCG.Action.PURSUIT:
			return WorldState.has_crimes()
		_:
			return true
	
func calculate_weight(resource_type: PCG.ResourceType) -> float:
	var chosen_resource: BaseResource

	if resource_type == PCG.ResourceType.TOTAL_FOOD:
		chosen_resource = WorldState.resource_manager.total_food
	else:
		chosen_resource = WorldState.resource_manager.get_resource(npc, resource_type)

	var deficiency_point = chosen_resource.get_deficiency_point()
	var variance = chosen_resource.amount - deficiency_point
	
	var upper_bound = (
		chosen_resource.upper_threshold
		if chosen_resource.is_unbounded()
		else chosen_resource.max_value
	)
	
	var ratio = variance / (upper_bound - deficiency_point)
	var weighted_score = 1 - clamp(ratio, 0, 1)
	return weighted_score * weight
