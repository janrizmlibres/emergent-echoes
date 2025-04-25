@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var target: Actor = blackboard.get_value("data").target
  return SUCCESS if target != null else FAILURE