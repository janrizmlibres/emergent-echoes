@tool
extends ActionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  blackboard.set_value("move_position", WorldState._shop.global_position)
  return SUCCESS
