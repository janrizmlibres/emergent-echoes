@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  var target: Actor = blackboard.get_value("data").target
  target.handle_detainment(actor)
  return SUCCESS