@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var target_found = blackboard.get_value("target_found")
  return SUCCESS if target_found else FAILURE