class_name BaseTrait
extends Node

@export_range(0.01, 1, 0.01)
var weight: float = 0.5

@export var actor_node: NPC

var action_candidates: Array[Dictionary] = []

func evaluation_action(social_practice: Globals.SocialPractice) -> Array[Dictionary]:
	action_candidates.clear()

	match social_practice:
		Globals.SocialPractice.PROACTIVE:
			evaluation_proactive_action()

	return action_candidates.duplicate()

func evaluation_proactive_action():
	pass

func get_actor_candidates(resource_type: Globals.ResourceType) -> Array[Actor]:
	var candidates: Array[Actor] = []
	var peer_actors: Array[Actor] = WorldState.get_peer_actors(actor_node).duplicate()
	peer_actors.shuffle()
	
	for actor in peer_actors:
		if actor is Player and randf() > 0.2: continue
		if not actor.holds_resource(resource_type): continue

		if WorldState.is_actor_valid_target(actor_node, actor):
			candidates.append(actor)
	
	return candidates

func add_action(action: Globals.Action, resource_type: Globals.ResourceType, data = {}) -> void:
	var action_weight: float = calculate_weight(resource_type)
	var action_data = {"action": action, "data": data}
	action_candidates.append({
		"action_data": action_data,
		"weight": action_weight
	})

func calculate_weight(resource_type: Globals.ResourceType) -> float:
	var chosen_resource: ResourceStat = actor_node.get_resource(resource_type)
	
	var deficiency_point = chosen_resource.get_deficiency_point()
	print("Skewed DP for ", actor_node.name, " for ", Globals.get_resource_string(resource_type),
		" under trait ", name, ": ", deficiency_point)
	var variance = chosen_resource.amount - deficiency_point
	
	var upper_bound = (
		chosen_resource.upper_threshold
		if chosen_resource.is_unbounded()
		else ResourceStat.MAX_VALUE[chosen_resource.type]
	)
	
	var ratio = variance / (upper_bound - deficiency_point)
	var weighted_score = 1 - clamp(ratio, 0, 1)
	return weighted_score * weight
