@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var action = blackboard.get_value("action")
  return SUCCESS if not action == PCG.Action.WANDER else FAILURE
