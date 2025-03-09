@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var target = blackboard.get_value("target")
  return SUCCESS if WorldState.actor_state[target].is_busy else FAILURE