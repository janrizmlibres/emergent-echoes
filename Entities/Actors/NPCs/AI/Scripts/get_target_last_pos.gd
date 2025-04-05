@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  var target = blackboard.get_value("target")
  var target_last_pos = (actor as NPC).memorizer.get_last_known_position(target)
  blackboard.set_value("move_position", target_last_pos)
  return SUCCESS