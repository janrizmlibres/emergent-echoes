class_name FarmerTrait
extends BaseTrait

func evaluation_proactive_action():
	add_action(PCG.Action.PLANT, PCG.ResourceType.TOTAL_FOOD)
	add_action(PCG.Action.HARVEST, PCG.ResourceType.TOTAL_FOOD)