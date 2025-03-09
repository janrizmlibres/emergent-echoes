@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var action_pending: bool = blackboard.get_value("action_pending")
  return SUCCESS if action_pending == true else FAILURE