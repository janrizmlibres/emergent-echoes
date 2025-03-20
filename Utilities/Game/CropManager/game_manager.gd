extends Node

@onready var checkpoint = 0
@onready var crop_matured = false
@onready var are_there_crops = false
@onready var farmer_death_location
@onready var toblin = get_tree().get_nodes_in_group("Toblin")
@onready var garreth = get_tree().get_nodes_in_group("Garreth")
@onready var silas = get_tree().get_nodes_in_group("Silas")
@onready var player = get_tree().get_nodes_in_group("Player")
@onready var hud = get_tree().get_nodes_in_group("Hud")

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
	hud[1].text = hud[1].text + "\n " + actor_name + " has perished from hunger"
	
func set_total_food(food_amount):
	hud[0].text = "Total Food In Market: " + food_amount
