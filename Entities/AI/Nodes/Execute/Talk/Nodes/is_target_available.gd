@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var target = blackboard.get_value("target")

  if WorldState.actor_state[target].is_available:
    return SUCCESS

  blackboard.set_value("target_found", false)
  return FAILURE