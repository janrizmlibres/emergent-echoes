@tool
extends ConditionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  var target = blackboard.get_value("target")
  return SUCCESS if (actor as NPC).actors_in_range.has(target) else FAILURE