@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var target_detained = blackboard.get_value("target_detained")
  return SUCCESS if target_detained else FAILURE