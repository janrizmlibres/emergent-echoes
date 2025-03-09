@tool
extends ActionLeaf

func tick(actor: Node, _blackboard: Blackboard) -> int:
  (actor as NPC).animation_state.travel("Eat")
  return RUNNING