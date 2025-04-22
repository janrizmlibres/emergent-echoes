class_name Strategiser
extends Node

var npc: NPC
var npc_agent: NPCAgent
var eval_timer := Timer.new()

func _init(_npc: NPC, agent: NPCAgent) -> void:
	npc = _npc
	npc_agent = agent

func _ready():
	eval_timer.one_shot = true
	eval_timer.timeout.connect(on_eval_timer_timeout)
	add_child(eval_timer)
	start_timer()

func evaluation_action(practice: PCG.SocialPractice) -> ActionData:
	var candidates: Array[ActionCandidate] = []

	for trait_mod in WorldState.npc_manager.get_traits(npc):
		var action_candidates: Array[ActionCandidate] = trait_mod.evaluation_action(practice)

		for candidate in action_candidates:
			if randf() <= candidate.weight:
				candidates.append(candidate)

	if candidates.is_empty(): return null

	var max_weight: float = 0.0
	for candidate in candidates:
		max_weight = max(max_weight, candidate.weight)

	var max_weighted_actions = []
	for candidate in candidates:
		if absf(candidate.weight - max_weight) < 0.01:
			max_weighted_actions.append(candidate)
  
	return max_weighted_actions[randi() % max_weighted_actions.size()].action_data

func start_timer():
	var interval := npc_agent.evaluation_interval
	eval_timer.start(randf_range(interval.x, interval.y))

func stop_timer():
	eval_timer.stop()

func on_eval_timer_timeout() -> void:
	var action_data := evaluation_action(PCG.SocialPractice.PROACTIVE)

	if action_data == null:
		print("No action data evaluated.")
		start_timer()
	else:
		npc_agent.emit_action_evaluated(action_data)
