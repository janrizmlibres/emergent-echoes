@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var action = blackboard.get_value("action")
  return SUCCESS if action == Globals.Action.INTERACT else FAILURE
