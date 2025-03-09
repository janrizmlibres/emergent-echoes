@tool
extends ConditionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  var interaction_required = blackboard.get_value("interaction_required")

  if interaction_required:
    WorldState.actor_state[actor as NPC].current_action = Globals.Action.INTERACT
    return SUCCESS

  return FAILURE