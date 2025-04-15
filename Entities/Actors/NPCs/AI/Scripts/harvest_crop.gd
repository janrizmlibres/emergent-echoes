@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  var npc = actor as NPC
  var anim_finished = blackboard.get_value("anim_finished")
  
  if anim_finished:
    blackboard.set_value("anim_finished", false)
    return SUCCESS

  npc.animation_state.travel("Harvest")
  return RUNNING

func interrupt(actor: Node, _blackboard: Blackboard) -> void:
  WorldState._actor_state[actor as NPC].is_busy = false

func before_run(actor: Node, _blackboard: Blackboard) -> void:
  WorldState._actor_state[actor as NPC].is_busy = true

func after_run(actor: Node, _blackboard: Blackboard) -> void:
  WorldState._actor_state[actor as NPC].is_busy = false
  WorldState.total_food.amount += 1
  WorldState._shop.food_amount += 1
