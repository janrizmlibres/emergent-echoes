extends Node

class ActorState:
	var current_action: Globals.Action = Globals.Action.NONE
	var current_petition_resource: Globals.ResourceType = Globals.ResourceType.NONE
	var is_busy: bool = false
	var is_available: bool = true

var global_events: Array[String] = []
var actor_state: Dictionary[Actor, ActorState] = {}
var crimes: Array = [Crime]

var total_food: ResourceStat

func initialize(_total_food: ResourceStat, actors: Array) -> void:
	total_food = _total_food

	for actor in actors:
		var peer_actors = actors.duplicate()
		peer_actors.erase(actor)
		actor.memorizer.initialize(peer_actors)
		
		actor_state[actor] = ActorState.new()
		total_food.amount += actor.get_resource_amount(Globals.ResourceType.FOOD)

func get_peer_actors(actor: Actor) -> Array[Actor]:
	var peer_actors = actor_state.keys().duplicate()
	peer_actors.erase(actor)
	return peer_actors

func get_actor_count() -> int:
	return actor_state.size()

func is_actor_valid_target(initiator: Actor, target: Actor) -> bool:
	var target_last_position = initiator.memorizer.get_last_known_position(target)
	if target_last_position == Vector2.INF and not initiator.actors_in_range.has(target):
		return false

	if not actor_state[target].is_available: return false
	return true

func is_actor_busy(actor: Actor) -> bool:
	return actor_state[actor].is_busy

func record_crime(crime) -> void:
	crimes.append(crime)
