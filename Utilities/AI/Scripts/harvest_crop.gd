@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  if blackboard.get_value("harvest_finished"):
    blackboard.set_value("harvest_finished", false)
    return SUCCESS

  actor.animation_state.travel("Harvest")
  return RUNNING

func interrupt(actor: Node, _blackboard: Blackboard) -> void:
  WorldState.set_status(actor, ActorState.State.FREE)

func before_run(actor: Node, _blackboard: Blackboard) -> void:
  WorldState.set_status(actor, ActorState.State.OCCUPIED)

func after_run(actor: Node, _blackboard: Blackboard) -> void:
  WorldState.set_status(actor, ActorState.State.FREE)
  WorldState.resource_manager.total_food.amount += 1
  WorldState.resource_manager.modify_resource(actor, PCG.ResourceType.MONEY, 10)
  WorldState.shop.food_amount += 1
