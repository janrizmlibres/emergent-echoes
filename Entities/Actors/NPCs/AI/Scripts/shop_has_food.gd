@tool
extends ConditionLeaf

func tick(_actor: Node, _blackboard: Blackboard) -> int:
  return SUCCESS if WorldState._shop.food_amount > 0 else FAILURE