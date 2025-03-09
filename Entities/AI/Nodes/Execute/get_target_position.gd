@tool
extends ActionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var target: Actor = blackboard.get_value("target")
  blackboard.set_value("move_position", target.global_position)
  return SUCCESS
