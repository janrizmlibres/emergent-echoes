@tool
extends ActionLeaf

func tick(actor: Node, _blackboard: Blackboard) -> int:
	var npc = actor as NPC

	var quantity = floori(get_quantity(npc))
	WorldState._shop.food_amount -= quantity

	npc.modify_resource(PCG.ResourceType.FOOD, quantity)
	npc.modify_resource(PCG.ResourceType.MONEY, -quantity * 10)
	npc.float_text_controller.show_float_text(
		PCG.ResourceType.FOOD,
		str(quantity),
		true
	)
	return SUCCESS

func get_quantity(npc):
	var food_resource = npc.get_resource(PCG.ResourceType.FOOD)
	var money_resource = npc.get_resource(PCG.ResourceType.MONEY)

	var deficiency_point = food_resource.get_deficiency_point()

	var desired_quantity = deficiency_point - food_resource.amount
	var can_buy = min(money_resource.amount / 10, WorldState._shop.food_amount)
	can_buy = min(can_buy, 10)
	return clamp(desired_quantity, 1, can_buy)
