@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  var target_candidates = (actor as NPC).actors_in_range.duplicate()

  for target in target_candidates:
    if target.is_valid_target():
      blackboard.set_value("target", target)
      return SUCCESS
  
  return FAILURE