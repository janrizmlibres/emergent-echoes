extends ActionLeaf

@onready var float_text_controller = $"../../../../FloatTextController"

signal move_actor(set_state: String, patrol_location: Vector2)

func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") != "go to prison":
		return FAILURE
	
	move_actor.emit(Vector2(159, 255))
	
	if blackboard.get_value("agent_arrived") == true:
		_actor.get_node("CarryProp").hide_sprite()
		GameManager.player[0].global_position = Vector2(201, 256)
		GameManager.player[0].visible = true
		GameManager.player[0].stop_interaction()
		
		_actor.remove_child(GameManager.player[0].remote_transform)
		GameManager.player[0].add_child(GameManager.player[0].remote_transform)
		
		GameManager.set_total_food("0")
		
		float_text_controller.show_float_text(Globals.ResourceType.SATIATION, "10", false)
		
		GameManager.cutscene_market()
		blackboard.set_value("agent_arrived", false)
		
		return SUCCESS
			
	return RUNNING
