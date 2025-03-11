@tool
extends ActionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  blackboard.set_value("target_reached", true)
  return SUCCESS