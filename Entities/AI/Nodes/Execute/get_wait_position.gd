@tool
extends ActionLeaf

@export var wait_distance: float = 40

func tick(actor: Node, blackboard: Blackboard) -> int:
  var target: Actor = blackboard.get_value("target")
  var actor_node = actor as Actor
  assert(actor_node != null, "Actor must be an Actor")

  var direction = target.global_position.direction_to(actor_node.global_position)
  blackboard.set_value("move_position", target.global_position + direction * wait_distance)

  return SUCCESS