@tool
extends ConditionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var target: Actor = blackboard.get_value("target")
  if target.is_queued_for_deletion(): return FAILURE
  if not is_instance_valid(target): return FAILURE
  return SUCCESS if target.is_valid_target() else FAILURE
