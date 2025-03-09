@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  var target_candidates = (actor as NPC).actors_in_range

  for target in target_candidates:
    if WorldState.actor_state[target].is_available:
      blackboard.set_value("target", target)
      blackboard.set_value("target_found", true)
      return SUCCESS
  
  return SUCCESS