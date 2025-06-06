@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  var target: Actor = blackboard.get_value("data").target
  target.handle_detainment(actor)
  WorldState.set_status(target, ActorState.State.CAPTURED)
  blackboard.set_value("target_detained", true)
  return SUCCESS