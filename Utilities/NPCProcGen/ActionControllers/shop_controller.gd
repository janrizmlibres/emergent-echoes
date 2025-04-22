class_name ShopController

static func execute(npc: NPC) -> int:
	var res_mgr := WorldState.resource_manager
	var food_resource := res_mgr.get_resource(npc, PCG.ResourceType.FOOD)
	var money_resource := res_mgr.get_resource(npc, PCG.ResourceType.MONEY)

	var deficiency_point = food_resource.get_deficiency_point()

	var desired_quantity = deficiency_point - food_resource.amount
	var can_buy = min(money_resource.amount / 10, WorldState.shop.food_amount)
	can_buy = min(can_buy, 10)

	var quantity = floori(clamp(desired_quantity, 1, can_buy))
	WorldState.shop.food_amount -= quantity

	res_mgr.modify_resource(npc, PCG.ResourceType.FOOD, quantity)
	res_mgr.modify_resource(npc, PCG.ResourceType.MONEY, -quantity * 10)
	return quantity
