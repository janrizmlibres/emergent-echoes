@tool
extends ConditionLeaf

func tick(actor: Node, _blackboard: Blackboard) -> int:
	var amount := WorldState.resource_manager.get_resource_amount(
		actor,
		PCG.ResourceType.MONEY
	)
	
	if amount > 10:
		return SUCCESS

	return FAILURE
