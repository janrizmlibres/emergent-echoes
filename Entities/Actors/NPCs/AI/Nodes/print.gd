@tool
extends ActionLeaf

@export var text: String = ""

func tick(_actor: Node, _blackboard: Blackboard) -> int:
  print(text)
  return SUCCESS