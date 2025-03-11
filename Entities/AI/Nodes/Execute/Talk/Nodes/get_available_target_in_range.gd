@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  var target_candidates = (actor as NPC).actors_in_range

  for target in target_candidates:
    if WorldState.is_available(target):
      blackboard.set_value("target", target)
      return SUCCESS
  
  return FAILURE