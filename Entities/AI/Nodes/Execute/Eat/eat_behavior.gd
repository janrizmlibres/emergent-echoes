@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  var eat_finished = blackboard.get_value("eat_finished")
  
  if eat_finished == true:
    blackboard.set_value("eat_finished", false)
    blackboard.set_value("action_pending", false)
    return SUCCESS

  (actor as NPC).animation_state.travel("Eat")
  return RUNNING

func interrupt(actor: Node, _blackboard: Blackboard) -> void:
  WorldState.actor_state[actor as NPC].is_busy = false

func before_run(actor: Node, _blackboard: Blackboard) -> void:
  print(actor.name + " started eating")
  WorldState.actor_state[actor as NPC].is_busy = true

func after_run(actor: Node, _blackboard: Blackboard) -> void:
  print(actor.name + "stopped eating")
  WorldState.actor_state[actor as NPC].is_busy = false
