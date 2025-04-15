class_name MemoryManager
extends Node

var _memories: Dictionary[Actor, Dictionary] = {}

func register_actor(actor: Actor) -> void:
	assert(actor not in _memories, "MemoryManager: Actor already registered.")

	var actor_container = Node.new()
	actor_container.name = actor.name
	add_child(actor_container)

	_memories[actor] = {}

	for peer_actor in _memories.keys():
		actor_container.add_child(store_data(actor, peer_actor))
		get_node(peer_actor.name).add_child(store_data(peer_actor, actor))

func store_data(actor: Actor, query: Actor):
	_memories[actor][query] = ActorData.new()
	_memories[actor][query].last_known_position = query.global_position
	return _memories[actor][query]

func is_friendly(actor: Actor, query: Actor) -> bool:
	return _memories[actor][query].relationship >= 5

func is_trusted(actor: Actor, query: Actor) -> bool:
	return _memories[actor][query].relationship >= 15

func is_close(actor: Actor, query: Actor) -> bool:
	return _memories[actor][query].relationship >= 25

func get_relationship(actor: Actor, query: Actor) -> float:
	return _memories[actor][query].relationship

func modify_relationship(actor: Actor, query: Actor, amount: int) -> void:
	_memories[actor][query].relationship += amount

func get_last_known_position(actor: Actor, query: Actor) -> Vector2:
	return _memories[actor][query].last_known_position

func set_last_known_position(actor: Actor, query: Actor, position: Vector2) -> void:
	if actor not in _memories or query not in _memories[actor]:
		print("MemoryManager: Actor or query not registered.")
		return

	_memories[actor][query].last_known_position = position
