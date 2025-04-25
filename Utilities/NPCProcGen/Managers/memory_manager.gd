class_name MemoryManager
extends Node

var _memories: Dictionary[Actor, Dictionary] = {}

func register_actor(actor: Actor) -> void:
	assert(actor not in _memories, "MemoryManager: Actor already registered.")

	var actor_container = Node.new()
	actor_container.name = actor.name
	add_child(actor_container)

	var peer_actors := _memories.keys()
	_memories[actor] = {}

	for peer_actor in peer_actors:
		store_data(actor, peer_actor, actor_container)
		store_data(peer_actor, actor, get_node(peer_actor.name as NodePath))
	
func unregister_actor(actor: Actor) -> void:
	get_node(NodePath(actor.name)).queue_free()
	_memories.erase(actor)

func store_data(actor: Actor, query: Actor, container: Node) -> void:
	_memories[actor][query] = ActorData.new()
	_memories[actor][query].name = query.name
	container.add_child(_memories[actor][query])

func is_friendly(actor: Actor, query: Actor) -> bool:
	return _memories[actor][query].relationship >= 10

func is_trusted(actor: Actor, query: Actor) -> bool:
	return _memories[actor][query].relationship >= 20

func is_close(actor: Actor, query: Actor) -> bool:
	return _memories[actor][query].relationship >= 30

func get_relationship(actor: Actor, query: Actor) -> float:
	return _memories[actor][query].relationship

func modify_relationship(actor: Actor, query: Actor, amount: int) -> void:
	_memories[actor][query].relationship += amount