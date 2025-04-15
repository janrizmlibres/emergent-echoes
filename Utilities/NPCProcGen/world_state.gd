extends Node

var npc_manager := NPCManager.new()
var resource_manager := ResourceManager.new()
var memory_manager := MemoryManager.new()

var global_events: Array[String] = []

var _actor_state: Dictionary[Actor, ActorState] = {}
var _crimes: Array[Crime] = []

var _prisons: Array[Prison] = []
var _crops: Array[CropTile] = []
var _shop: Shop

func _ready():
	var state_container = Node.new()
	state_container.name = "ActorStates"
	add_child(state_container)

	npc_manager.name = "NPCManager"
	resource_manager.name = "ResourceManager"
	memory_manager.name = "MemoryManager"
	add_child(npc_manager)
	add_child(resource_manager)
	add_child(memory_manager)

func register_actor(actor: Actor) -> void:
	var pcg_agent := get_pcg_agent(actor)
	assert(pcg_agent != null, "ActorState: Agent node not found in actor node tree.")

	_actor_state[actor] = ActorState.new()
	_actor_state[actor].name = actor.name
	get_node("ActorStates").add_child(_actor_state[actor])

	if pcg_agent is NPCAgent:
		assert(actor is NPC, "NPCManager: Actor is not NPC.")
		npc_manager.register_npc(actor, pcg_agent)

	resource_manager.register_actor(actor, pcg_agent)
	memory_manager.register_actor(actor)

func get_pcg_agent(root: Node) -> Node:
	for child in root.get_children():
		if child is PCGAgent:
			return child
	return null
	
func register_prison(prison: Prison) -> void:
	_prisons.append(prison)

func register_crop(crop: CropTile) -> void:
	_crops.append(crop)

func register_shop(shop_arg: Shop) -> void:
	_shop = shop_arg
	resource_manager.total_food.amount += _shop.food_amount

func get_peer_actors(actor: Actor) -> Array[Actor]:
	var peer_actors := _actor_state.keys().duplicate()
	peer_actors.erase(actor)
	return peer_actors

func get_actor_count() -> int:
	return _actor_state.size()

func get_current_action(actor):
	return _actor_state[actor].current_action

func set_current_action(actor, action):
	_actor_state[actor].current_action = action

func actor_in_status(actor, status) -> bool:
	return _actor_state[actor].status == status

func is_trackable(initiator: Actor, target: Actor) -> bool:
	var target_last_position = initiator.memorizer.get_last_known_position(target)
	if target_last_position != Vector2.INF: return true
	if initiator.actors_in_range.has(self): return true
	return false

func is_valid_target(target: Actor) -> bool:
	if target.is_queued_for_deletion(): return false

	var status := _actor_state[target].status
	if status == ActorState.State.FREE:
		return true
	return status == ActorState.State.OCCUPIED

func is_actor_lawful(actor: Actor) -> bool:
	if actor is Player: return false
	return (actor as NPC).lawful_trait != null
	# ! TODO: This is a temporary solution, we need to check if the actor is a lawful NPC

func has_crimes() -> bool:
	return _crimes.size() > 0

func some_crop_in_status(status: CropTile.Status) -> bool:
	for crop in _crops:
		if crop.status == status:
			return true
	return false

func get_open_case(investigator: NPC) -> Crime:
	for crime in _crimes:
		if not crime.is_open(): continue
		crime.investigator = investigator
		
		if crime.participants.has(investigator):
			crime.verifiers.append(investigator)
		
		return crime

	return null

func queue_free_actor(actor: Actor):
	_actor_state.erase(actor)

	for peer in get_peer_actors(actor):
		peer.memorizer.actor_data.erase(actor)
		peer.actors_in_range.erase(actor)

	for crime in _crimes:
		if not crime.participants.has(actor): continue
		if crime.verifiers.has(actor): continue
		if not crime.falsifiers.has(actor):
			crime.falsifiers.append(actor)
