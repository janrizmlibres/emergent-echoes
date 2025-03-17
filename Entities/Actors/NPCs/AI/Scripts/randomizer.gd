@tool
extends ConditionLeaf

@export var probability: float = 0.5

func tick(_actor: Node, _blackboard: Blackboard) -> int:
  return SUCCESS if randf() <= probability else FAILURE