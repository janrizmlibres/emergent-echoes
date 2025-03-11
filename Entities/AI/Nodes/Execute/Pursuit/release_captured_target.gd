@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  var npc = actor as NPC
  var data: Actor = blackboard.get_value("data")
  var prison_marker = blackboard.get_value("prison_marker")
  var target = data.target

  npc.carry_prop.hide_sprite()
  target.global_position = prison_marker.global_position
  target.visible = true

  if target is Player:
    target.collision_mask = 0b0011
  else:
    (target as NPC).executor.set_enable(true)

  data.crime.status = Crime.Status.SOLVED
  npc.lawful_trait.assigned_case = null
  npc.executor.end_action()
  return SUCCESS