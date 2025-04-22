@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  if blackboard.get_value("eat_finished"):
    return SUCCESS

  actor.animation_state.travel("Eat")
  return RUNNING

func interrupt(actor: Node, _blackboard: Blackboard) -> void:
  WorldState.set_status(actor, ActorState.State.FREE)

func before_run(actor: Node, _blackboard: Blackboard) -> void:
  WorldState.set_status(actor, ActorState.State.OCCUPIED)

func after_run(actor: Node, _blackboard: Blackboard) -> void:
  WorldState.set_status(actor, ActorState.State.FREE)
  WorldState.total_food.amount -= 1
  WorldState.resource_manager.modify_resource(actor, PCG.ResourceType.SATIATION, 20)
  WorldState.resource_manager.modify_resource(actor, PCG.ResourceType.FOOD, -1)
  actor.set_main_state(NPC.MainState.WANDER)
