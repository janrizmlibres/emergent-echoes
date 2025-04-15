@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
	var npc = actor as NPC
	var target = blackboard.get_value("target")
	var data = blackboard.get_value("data")
	var resource_type = data.resource_type

	var actor_resource = npc.get_resource(resource_type)
	var target_resource = target.get_resource(resource_type)

	var deficiency_point = actor_resource.get_deficiency_point()
	var max_excess = (actor_resource.get_max_value() - deficiency_point) * npc.thief_trait.weight
	var desired_amount = deficiency_point + randf() * max_excess;
	var steal_amount = max(1, desired_amount - actor_resource.amount);
	steal_amount = min(steal_amount, target_resource.amount)
	steal_amount = floor(min(steal_amount, 100))

	target.give_resource(npc, resource_type, steal_amount)

	var participants: Array = npc.actors_in_range.duplicate()
	WorldState._crimes.append(Crime.new(Crime.Category.THEFT, npc, participants))
	print("Open cases: " + str(WorldState._crimes.filter(func(x): return x.is_open()).size()))

	npc.float_text_controller.show_float_text(
		resource_type,
		str(steal_amount),
		false
	)
	return SUCCESS
