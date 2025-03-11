@tool
extends ActionLeaf

func tick(_actor: Node, blackboard: Blackboard) -> int:
  var prison_nodes = get_tree().get_nodes_in_group("Prisons")
  
  for prison in prison_nodes:
    var marker = prison as PrisonMarker
    if marker.current_capacity > 0:
      marker.current_capacity -= 1
      blackboard.set_value("move_position", marker.global_position)
      blackboard.set_value("prison_marker", marker)
  
  return SUCCESS