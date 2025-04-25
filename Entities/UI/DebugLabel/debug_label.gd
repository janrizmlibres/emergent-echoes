extends Label

@export var actor: Actor

func _process(_delta):
	update_label()

func update_label():
	var label_string = ""

	if actor.holds_resource(PCG.ResourceType.MONEY):
		var resource_amount = actor.get_resource_amount(PCG.ResourceType.MONEY)
		label_string += "Money: " + str(resource_amount) + "\n"

	if actor.holds_resource(PCG.ResourceType.FOOD):
		var resource_amount = actor.get_resource_amount(PCG.ResourceType.FOOD)
		label_string += "Food: " + str(resource_amount) + "\n"
	
	if actor.holds_resource(PCG.ResourceType.SATIATION):
		var resource_amount = actor.get_resource_amount(PCG.ResourceType.SATIATION)
		label_string += "Satiation: " + str(round(resource_amount)) + "\n"

	if actor.holds_resource(PCG.ResourceType.COMPANIONSHIP):
		var resource_amount = actor.get_resource_amount(PCG.ResourceType.COMPANIONSHIP)
		label_string += "Companionship: " + str(round(resource_amount)) + "\n"

	if actor.holds_resource(PCG.ResourceType.DUTY):
		var resource_amount = actor.get_resource_amount(PCG.ResourceType.DUTY)
		label_string += "Duty: " + str(round(resource_amount)) + "\n"

	text = label_string.strip_edges()
