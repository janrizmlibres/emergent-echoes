@tool
extends ConditionLeaf

func tick(actor: Node, _blackboard: Blackboard) -> int:
  var npc = actor as NPC
  
  if npc.get_resource_amount(Globals.ResourceType.MONEY) > 10:
    return SUCCESS

  return FAILURE