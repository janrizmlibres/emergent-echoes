@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var npc = actor as NPC
	var target = blackboard.get_value("target")
	var shop = blackboard.get_value("shop")

	var quantity = get_quantity(target)
	shop.food_amount -= quantity
	npc.modify_resource(Globals.ResourceType.FOOD, quantity)
	npc.float_text_controller.show_float_text(
		Globals.ResourceType.FOOD,
		str(quantity),
		true
	)
	return SUCCESS

func get_quantity(target):
	var food_resource = target.get_resource(Globals.ResourceType.FOOD)
	var money_resource = target.get_resource(Globals.ResourceType.MONEY)

	var deficiency_point = food_resource.get_deficiency_point()

	var desired_quantity = deficiency_point - food_resource.amount
	var can_buy = money_resource.amount / 10
	return clamp(desired_quantity, 1, can_buy)