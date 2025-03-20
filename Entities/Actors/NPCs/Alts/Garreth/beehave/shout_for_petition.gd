extends ActionLeaf

@onready var timer = $"../../../../Timer"
@onready var emote_bubble = $"../../../../EmoteBubble"

@onready var animation_tree = $"../../../../AnimationTree"
@onready var animation_state: AnimationNodeStateMachinePlayback = animation_tree.get("parameters/playback")

@onready var blackboard = $"../../../../Blackboard"

var chances = 0

func before_run(actor: Node, blackboard: Blackboard) -> void:
	if blackboard.get_value("current_state") == "shouting" || blackboard.get_value("current_state") == "surveying":
		timer.start()
		return

func tick(actor: Node, blackboard: Blackboard) -> int:
	if blackboard.get_value("current_state") != "shouting" && blackboard.get_value("current_state") != "surveying":
		return FAILURE
	
	animation_state.travel("Idle")
	
	if chances >= 5:
		timer.stop()
		blackboard.set_value("current_state", "patrolling")
		chances = 0
		return SUCCESS
			
	return RUNNING

func _on_timer_timeout() -> void:
	if blackboard.get_value("current_state") != "shouting" && blackboard.get_value("current_state") != "surveying":
		return
		
	timer.start()
	emote_bubble.show_emote_bubble(Globals.Emote.EXCLAMATION)
	chances += 1
	pass # Replace with function body.
