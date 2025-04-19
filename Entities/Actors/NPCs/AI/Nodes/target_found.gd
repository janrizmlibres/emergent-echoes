@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var data: Dictionary = blackboard.get_value("data")
  return SUCCESS if data.has("target") else FAILURE