class_name NPCManager
extends Node

var _traits: Dictionary[NPC, Dictionary] = {}
var evaluators: Dictionary[NPC, Evaluator] = {}

func register_npc(npc: NPC, npc_agent: NPCAgent):
	assert(npc not in _traits, "NPCManager: NPC already registered")

	var npc_container = Node.new()
	npc_container.name = npc.name
	add_child(npc_container)

	npc_container.add_child(setup_traits(npc, npc_agent))
	
	evaluators[npc] = Evaluator.new(npc, npc_agent)
	evaluators[npc].name = "Evaluator"
	npc_container.add_child(evaluators[npc])

func unregister_npc(npc: NPC):
	if has_trait(npc, "lawful"):
		var lawful_mod: LawfulTrait = get_trait(npc, "lawful")
		
		if lawful_mod.current_case != null:
			lawful_mod.current_case.cleanse_actor(npc)
			lawful_mod.current_case.reset()

	get_node(NodePath(npc.name)).queue_free()
	_traits.erase(npc)
	evaluators.erase(npc)

func setup_traits(npc: NPC, npc_agent: NPCAgent) -> Node:
	var traits_container = Node.new()
	traits_container.name = "Traits"

	_traits[npc] = {}
	add_trait(npc, SurvivalTrait.new(npc, npc_agent.survival), "survival", traits_container)

	if npc_agent.thief > 0:
		add_trait(npc, ThiefTrait.new(npc, npc_agent.thief), "thief", traits_container)

	if npc_agent.lawful > 0:
		add_trait(npc, LawfulTrait.new(npc, npc_agent.lawful), "lawful", traits_container)

	if npc_agent.farmer > 0:
		add_trait(npc, FarmerTrait.new(npc, npc_agent.farmer), "farmer", traits_container)
	
	return traits_container

func add_trait(npc: NPC, module: BaseTrait, trait_name: String, container: Node):
	_traits[npc][trait_name] = module
	_traits[npc][trait_name].name = trait_name.capitalize()
	container.add_child(_traits[npc][trait_name])

func get_trait(npc: NPC, trait_name: String) -> BaseTrait:
	return _traits[npc][trait_name]

func get_traits(npc: NPC) -> Array:
	return _traits[npc].values()

func get_traits_as_text(npc: NPC) -> Array:
	return _traits[npc].keys()

func get_npcs_with_trait(trait_name: String) -> Array:
	return _traits.keys().filter(func(n):
		return has_trait(n, trait_name)
	)

func end_case(npc: NPC):
	assert(has_trait(npc, "lawful"), "NPC must be Lawful")

	var lawful_mod: LawfulTrait = get_trait(npc, "lawful")
	lawful_mod.current_case = null

func run_evaluation(npc: NPC) -> void:
	evaluators[npc].start_timer()

func has_trait(npc: NPC, trait_name: String) -> bool:
	return _traits[npc].has(trait_name)
