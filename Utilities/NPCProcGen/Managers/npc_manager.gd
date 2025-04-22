class_name NPCManager
extends Node

var _traits: Dictionary[NPC, Dictionary] = {}
var evaluators: Dictionary[NPC, Strategiser] = {}

func register_npc(npc: NPC, npc_agent: NPCAgent):
	assert(npc not in _traits, "NPCManager: NPC already registered")

	var npc_container = Node.new()
	npc_container.name = npc.name
	add_child(npc_container)

	npc_container.add_child(setup_traits(npc, npc_agent))
	
	evaluators[npc] = Strategiser.new(npc, npc_agent)
	evaluators[npc].name = "Strategiser"
	npc_container.add_child(evaluators[npc])

func setup_traits(npc: NPC, npc_agent: NPCAgent) -> Node:
	var traits_container = Node.new()
	traits_container.name = "Traits"

	_traits[npc] = {}
	add_trait(npc, SurvivalTrait.new(npc, npc_agent.survival), "survival", traits_container)

	if npc_agent.thief > 0:
		add_trait(npc, ThiefTrait.new(npc, npc_agent.thief), "thief", traits_container)

	if npc_agent.lawful > 0:
		_traits[npc]["lawful"] = LawfulTrait.new(npc, npc_agent.lawful)
		_traits[npc]["lawful"].name = "Lawful"
		traits_container.add_child(_traits[npc]["lawful"])

	if npc_agent.farmer > 0:
		_traits[npc]["farmer"] = FarmerTrait.new(npc, npc_agent.farmer)
		_traits[npc]["farmer"].name = "Farmer"
		traits_container.add_child(_traits[npc]["farmer"])
	
	return traits_container

func add_trait(npc: NPC, module: BaseTrait, trait_name: String, container: Node):
	_traits[npc][trait_name] = module
	_traits[npc][trait_name].name = trait_name.capitalize()
	container.add_child(_traits[npc][trait_name])

func get_traits(npc: NPC) -> Array:
	return _traits[npc].values()

func run_evaluation(npc: NPC) -> void:
	evaluators[npc].start_timer()

func has_trait(npc: NPC, trait_name: String) -> bool:
	return _traits[npc].has(trait_name)