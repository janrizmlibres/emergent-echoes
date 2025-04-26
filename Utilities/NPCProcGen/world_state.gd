extends Node

var npc_manager := NPCManager.new()
var resource_manager := ResourceManager.new()
var memory_manager := MemoryManager.new()

var _actor_state: Dictionary[Actor, ActorState] = {}

var _pending_crimes: Array[Crime] = []

var _prisons: Array[Prison] = []
var _crops: Array[CropTile] = []
var shop: Shop

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
	$ActorStates.add_child(_actor_state[actor])

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
	shop = shop_arg
	resource_manager.total_food.amount += shop.food_amount

func get_peer_actors(actor: Actor) -> Array[Actor]:
	var peer_actors := _actor_state.keys().duplicate()
	peer_actors.erase(actor)
	return peer_actors

func get_actor_count() -> int:
	return _actor_state.size()

func get_player() -> Player:
	var actors := _actor_state.keys()
	var idx := actors.find_custom(func(a): return a is Player)
	return actors[idx] if idx != -1 else null

func get_current_action(actor):
	return _actor_state[actor].current_action

func set_current_action(actor, action):
	_actor_state[actor].current_action = action

func set_status(actor, status):
	_actor_state[actor].status = status

func actor_in_status(actor, status) -> bool:
	return _actor_state[actor].status == status

func is_interactable(target: Actor) -> bool:
	if not _actor_state.has(target):
		return false

	var status := _actor_state[target].status
	if status == ActorState.State.FREE:
		return true
	return status == ActorState.State.OCCUPIED

func is_busy(actor: Actor) -> bool:
	if not _actor_state.has(actor):
		return false

	return _actor_state[actor].status == ActorState.State.OCCUPIED

func is_interceptable(target: Actor) -> bool:
	if not _actor_state.has(target):
		return false

	return _actor_state[target].status != ActorState.State.CAPTURED

func add_pending_crime(crime: Crime):
	_pending_crimes.append(crime)

func get_pending_crime() -> Crime:
	return _pending_crimes.pop_front()

func get_available_prison() -> Prison:
	return _prisons.pick_random()

func has_actor(actor: Actor) -> bool:
	return _actor_state.has(actor)

func has_crimes() -> bool:
	return _pending_crimes.size() > 0

func some_crop_in_status(status: CropTile.Status) -> bool:
	for crop in _crops:
		if crop.status == status and !crop.is_attended:
			return true
	return false

func get_crop_in_status(status: CropTile.Status) -> CropTile:
	for crop in _crops:
		if crop.status == status and !crop.is_attended:
			return crop
	return null

func unregister_actor(actor: Actor):
	_actor_state[actor].queue_free()
	_actor_state.erase(actor)

	for crime in _pending_crimes:
		crime.cleanse_actor(actor)
	
	if actor is NPC:
		npc_manager.unregister_npc(actor)
	
	resource_manager.unregister_actor(actor)
	memory_manager.unregister_actor(actor)

# func get_open_case(investigator: NPC) -> Crime:
# 	for crime in _pending_crimes:
# 		if not crime.is_open(): continue
# 		crime.investigator = investigator
		
# 		if crime._participants.has(investigator):
# 			crime._verifiers.append(investigator)
		
# 		return crime

# 	return null
