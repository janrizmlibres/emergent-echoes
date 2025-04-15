class_name NPCManager
extends Node

var _traits: Dictionary[NPC, Dictionary] = {}
var _strategisers: Dictionary[NPC, Strategiser] = {}

func register_npc(npc: NPC, npc_agent: NPCAgent):
	assert(npc not in _traits, "NPCManager: NPC already registered")

	var npc_container = Node.new()
	npc_container.name = npc.name
	add_child(npc_container)

	npc_container.add_child(setup_traits(npc, npc_agent))
	
	_strategisers[npc] = Strategiser.new(npc, npc_agent)
	npc_container.add_child(_strategisers[npc])

func setup_traits(npc: NPC, npc_agent: NPCAgent) -> Node:
	var traits_container = Node.new()
	traits_container.name = "Traits"

	_traits[npc] = {}
	_traits[npc]["survival"] = SurvivalTrait.new(npc_agent.survival)
	traits_container.add_child(_traits[npc]["survival"])

	if npc_agent.thief > 0:
		_traits[npc]["thief"] = ThiefTrait.new(npc_agent.thief)
		traits_container.add_child(_traits[npc]["thief"])

	if npc_agent.lawful > 0:
		_traits[npc]["lawful"] = LawfulTrait.new(npc_agent.lawful)
		traits_container.add_child(_traits[npc]["lawful"])

	if npc_agent.farmer > 0:
		_traits[npc]["farmer"] = FarmerTrait.new(npc_agent.farmer)
		traits_container.add_child(_traits[npc]["farmer"])
	
	return traits_container

func run_npc(npc: NPC) -> void:
	assert(npc in _traits, "NPCManager: NPC not registered")
	_strategisers[npc].start_timer()

func is_lawful(npc: NPC) -> bool:
	return _traits[npc].has("lawful")