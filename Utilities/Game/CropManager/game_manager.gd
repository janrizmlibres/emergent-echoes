extends Node

var hud_interface: HUDInterfaceAlt = null

@onready var checkpoint = 0
@onready var crop_matured = false
@onready var are_there_crops = false
@onready var farmer_death_location
@onready var toblin = get_tree().get_nodes_in_group("Toblin")
@onready var garreth = get_tree().get_nodes_in_group("Garreth")
@onready var silas = get_tree().get_nodes_in_group("Silas")
@onready var player = get_tree().get_nodes_in_group("Player")
@onready var hud = get_tree().get_nodes_in_group("Hud")

var relationships: Dictionary[CharacterBody2D, Dictionary] = {}

func _ready():
	hud_interface = get_tree().get_first_node_in_group("HUD")

func cutscene_start():
	toblin[0].get_node("Blackboard").set_value("current_state", "interrupted")
	toblin[0].get_node("Blackboard").set_value("cutscene_state", "farmer is dead")

func cutscene_bonfire():
	toblin[0].get_node("Blackboard").set_value("cutscene_state", "go to bonfire")
	silas[0].get_node("Blackboard").set_value("cutscene_state", "go to bonfire")
	
func cutscene_market():
	toblin[0].get_node("Blackboard").set_value("cutscene_state", "go to market")
	silas[0].get_node("Blackboard").set_value("cutscene_state", "go to market")
	garreth[0].get_node("Blackboard").set_value("cutscene_state", "go to market")
	
func show_perished_message(actor_name):
	hud_interface.broadcast_event(actor_name + " died from hunger")
	
func set_total_food(food_amount):
	return

func register_actor(actor: CharacterBody2D) -> void:
	assert(actor not in relationships, "MemoryManager: Actor already registered.")

	var peer_actors := relationships.keys()
	relationships[actor] = {}

	for peer_actor in peer_actors:
		store_data(actor, peer_actor)
		store_data(peer_actor, actor)

func store_data(actor: CharacterBody2D, query: CharacterBody2D) -> void:
	var values = [0, 10, 20, 30]
	relationships[actor][query] = values.pick_random()
	
func get_peers(actor: CharacterBody2D) -> Array[CharacterBody2D]:
	var peer_actors := relationships.keys().duplicate()
	peer_actors.erase(actor)
	return peer_actors

func get_relationship(actor: CharacterBody2D, query: CharacterBody2D) -> float:
	return relationships[actor][query]
