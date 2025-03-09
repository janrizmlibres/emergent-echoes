@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var target_secured = blackboard.get_value("target_secured")
  return SUCCESS if target_secured == true else FAILURE