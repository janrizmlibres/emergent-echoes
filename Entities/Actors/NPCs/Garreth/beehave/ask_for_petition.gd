extends ActionLeaf

@onready var float_text_controller = $"../../../../FloatTextController"
@onready var emote_controller = $"../../../../EmoteController"

func before_run(actor: Node, blackboard: Blackboard) -> void:
	if blackboard.get_value("current_state") != "petitioning":
		return
		
	if blackboard.get_value("actor").get_name() == "Player":
		blackboard.get_value("actor").OnInteractionStartedOnNPCAlt(blackboard.get_value("current_state"), actor)
		actor.npc_active = false

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") == "petitioning":
		return RUNNING
	elif blackboard.get_value("current_state") == "petition answered":
		emote_controller.ShowEmoteBubble(5)
		float_text_controller.ShowFloatText(0, "100", true)
		blackboard.set_value("current_state", "done petitioning")
		return SUCCESS
	
	return FAILURE
