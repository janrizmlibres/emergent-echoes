@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  return SUCCESS if blackboard.get_value("assess_completed") else FAILURE