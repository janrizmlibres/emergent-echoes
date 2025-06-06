@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  var peers = WorldState.get_peer_actors(actor).filter(
    func(x): return WorldState.is_interactable(x)
  )

  if peers.is_empty():
    print_debug("Failed to get random target")
    return FAILURE

  var data = blackboard.get_value("data")
  data["target"] = peers.pick_random()
  blackboard.set_value("data", data)
  return SUCCESS