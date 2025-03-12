@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  var npc = actor as NPC
  var target: Actor = blackboard.get_value("target")

  npc.carry_prop.set_texture(target.name)
  npc.carry_prop.show_sprite()
  target.visible = false
  print("Target detained: ", target.name)

  if target is Player:
    target.collision_mask = 0b0001
  else:
    var target_npc = target as NPC
    target_npc.evaluation_timer.stop()
    target_npc.executor.set_enable(false)

  WorldState.set_captured(target, true)
  return SUCCESS