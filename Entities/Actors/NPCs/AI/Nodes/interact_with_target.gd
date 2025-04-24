@tool
extends ActionLeaf

enum Purpose {
	PETITION,
	TALK
}

@export var duration: float = 15
@export var purpose := Purpose.PETITION

var timer: float = duration

func tick(_actor: Node, _blackboard: Blackboard) -> int:
	timer -= get_process_delta_time()

	if timer <= 0:
		return SUCCESS

	return RUNNING

func interrupt(_actor: Node, blackboard: Blackboard) -> void:
	var target: Actor = blackboard.get_value("data").target
	if target.is_queued_for_deletion(): return
	target.stop_interaction()

func before_run(actor: Node, blackboard: Blackboard) -> void:
	var data: Dictionary = blackboard.get_value("data")

	if data.target is NPC:
		data.target.start_interaction(actor)
	elif purpose == Purpose.PETITION:
		var quantity := 5 if data.resource_type == PCG.ResourceType.MONEY else 1
		var amount := WorldState.resource_manager.get_resource_amount(
			data.target,
			data.resource_type
		)
		quantity = min(quantity, amount)

		EventManager.emit_show_npc_petition_hud(
			actor,
			data.resource_type,
			quantity
		)

func after_run(_actor: Node, blackboard: Blackboard) -> void:
	var target: Actor = blackboard.get_value("data").target
	target.stop_interaction()
