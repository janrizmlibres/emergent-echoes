@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var target = blackboard.get_value("target")

  if WorldState.is_available(target):
    return SUCCESS
  
  blackboard.erase_value("target")
  return FAILURE