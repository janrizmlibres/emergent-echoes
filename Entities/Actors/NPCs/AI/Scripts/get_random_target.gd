@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  var peers = WorldState.get_peer_actors(actor as Actor)
  peers = peers.filter(func(x): x.is_valid_target())

  if peers.is_empty(): return FAILURE

  blackboard.set_value("target", peers.pick_random())
  return SUCCESS