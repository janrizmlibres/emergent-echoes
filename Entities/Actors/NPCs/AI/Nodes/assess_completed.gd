@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var data = blackboard.get_value("data")
  return SUCCESS if data.assess_completed else FAILURE