@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var target_reached = blackboard.get_value("target_reached")
  return SUCCESS if target_reached else FAILURE
