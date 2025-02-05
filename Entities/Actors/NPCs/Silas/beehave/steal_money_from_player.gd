extends ActionLeaf 

func tick(actor: Node, blackboard: Blackboard) -> int:
	print("Silas stole money from the Player and is returning home.")
	return SUCCESS
