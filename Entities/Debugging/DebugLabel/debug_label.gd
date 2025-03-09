extends Label

@export var actor: Actor

func _process(_delta):
	update_label()

func update_label():
	var label_string = ""

	if actor.has_resource(Globals.ResourceType.MONEY):
		var resource_amount = actor.get_resource_amount(Globals.ResourceType.MONEY)
		label_string += "Money: " + str(resource_amount) + "\n"

	if actor.has_resource(Globals.ResourceType.FOOD):
		var resource_amount = actor.get_resource_amount(Globals.ResourceType.FOOD)
		label_string += "Food: " + str(resource_amount) + "\n"
	
	if actor.has_resource(Globals.ResourceType.SATIATION):
		var resource_amount = actor.get_resource_amount(Globals.ResourceType.SATIATION)
		label_string += "Satiation: " + str(round(resource_amount)) + "\n"

	if actor.has_resource(Globals.ResourceType.COMPANIONSHIP):
		var resource_amount = actor.get_resource_amount(Globals.ResourceType.COMPANIONSHIP)
		label_string += "Companionship: " + str(round(resource_amount)) + "\n"

	if actor.has_resource(Globals.ResourceType.DUTY):
		var resource_amount = actor.get_resource_amount(Globals.ResourceType.DUTY)
		label_string += "Duty: " + str(round(resource_amount)) + "\n"

	text = label_string.strip_edges()
