@tool
extends ActionLeaf

func tick(actor: Node, _blackboard: Blackboard) -> int:
  (actor as NPC).executor.end_action()
  return SUCCESS