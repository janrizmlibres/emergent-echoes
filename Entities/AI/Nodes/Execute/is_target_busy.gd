@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var target = blackboard.get_value("target")
  return SUCCESS if WorldState.is_actor_busy(target) else FAILURE