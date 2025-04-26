extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("cutscene_state") == "imprison the player":
		actor.get_node("CarryProp").set_texture("Player")
		actor.get_node("CarryProp").show_sprite()
		GameManager.player[0].visible = false
		
		GameManager.player[0].remove_child(GameManager.player[0].remote_transform)
		actor.add_child(GameManager.player[0].remote_transform)
		
		blackboard.set_value("cutscene_state", "go to prison")
		return SUCCESS
	
	return FAILURE
