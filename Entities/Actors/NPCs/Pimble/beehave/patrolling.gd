extends ActionLeaf

@onready var animation_tree = $"../../../../AnimationTree"
@onready var animation_state: AnimationNodeStateMachinePlayback = animation_tree.get("parameters/playback")

signal move_actor(set_state: String, patrol_location: Vector2)

var current_patrol_index: int = 0

const patrol_locations = [Vector2(517, 535), Vector2(508, 283), Vector2(250, 474), Vector2(737, 632)]	

func before_run(actor: Node, blackboard: Blackboard) -> void:
	if blackboard.get_value("current_state") == "patrolling":
		move_actor.emit(patrol_locations[current_patrol_index])
	
func tick(_actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "patrolling":
		return FAILURE
		
	if blackboard.get_value("agent_arrived") == true:
		if current_patrol_index < patrol_locations.size() - 1:
			current_patrol_index += 1
		else:
			current_patrol_index = 0
		
		CropManager.checkpoint = current_patrol_index
		animation_state.travel("Idle")
		blackboard.set_value("current_state", "idle")
		blackboard.set_value("agent_arrived", false)
		return SUCCESS
			
	return RUNNING
