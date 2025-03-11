@tool
extends ActionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  blackboard.erase_value("target")
  return SUCCESS