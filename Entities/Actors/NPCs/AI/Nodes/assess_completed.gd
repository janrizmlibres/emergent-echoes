@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var assess_completed: bool = blackboard.get_value("data").assess_completed
  return SUCCESS if assess_completed else FAILURE