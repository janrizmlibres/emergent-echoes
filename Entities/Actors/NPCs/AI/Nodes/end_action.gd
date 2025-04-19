@tool
extends ActionLeaf

func tick(actor: Node, _blackboard: Blackboard) -> int:
  actor.end_action()
  return SUCCESS
