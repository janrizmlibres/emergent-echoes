@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  return SUCCESS if blackboard.has_value("target") else FAILURE