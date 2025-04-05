extends Node

class ActorState:
	var current_action: Globals.Action = Globals.Action.NONE
	var is_busy: bool = false
	var is_available: bool = true
	var is_captured: bool = false

var global_events: Array[String] = []
var actor_state: Dictionary[Actor, ActorState] = {}
var crimes: Array[Crime] = []

var total_food: ResourceStat
var shop: Shop

func initialize(_total_food: ResourceStat, _shop: Shop, actors: Array) -> void:
	total_food = _total_food
	shop = _shop

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

func get_current_action(actor):
	return actor_state[actor].current_action

func set_current_action(actor, action):
	actor_state[actor].current_action = action

func is_busy(actor):
	return actor_state[actor].is_busy

func set_is_busy(actor, value):
	actor_state[actor].is_busy = value

func is_available(actor):
	return actor_state[actor].is_available

func set_availability(actor, value):
	actor_state[actor].is_available = value

func is_captured(actor):
	return actor_state[actor].is_captured

func set_captured(actor, value):
	actor_state[actor].is_captured = value

func get_open_case(investigator: NPC) -> Crime:
	for crime in crimes:
		if not crime.is_open(): continue
		crime.investigator = investigator
		
		if crime.participants.has(investigator):
			crime.verifiers.append(investigator)
		
		return crime

	return null

func queue_free_actor(actor: Actor):
	actor_state.erase(actor)

	for peer in get_peer_actors(actor):
		peer.memorizer.actor_data.erase(actor)
		peer.actors_in_range.erase(actor)

	for crime in crimes:
		if not crime.participants.has(actor): continue
		if crime.verifiers.has(actor): continue
		if not crime.falsifiers.has(actor):
			crime.falsifiers.append(actor)
