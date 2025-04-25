class_name TheftController

static func execute(npc: NPC, target: Actor, resource_type: PCG.ResourceType) -> int:
	var resource_mgr := WorldState.resource_manager
	var actor_resource := resource_mgr.get_resource(npc, resource_type)
	var target_resource := resource_mgr.get_resource(target, resource_type)

	var deficiency_point := actor_resource.get_deficiency_point()
	var thief_trait := WorldState.npc_manager.get_trait(npc, "thief")
	var max_excess = (actor_resource.max_value - deficiency_point) * thief_trait.weight
	var desired_amount = deficiency_point + randf() * max_excess;
	var steal_amount = max(1, desired_amount - actor_resource.amount);
	steal_amount = min(steal_amount, target_resource.amount)
	steal_amount = floor(min(steal_amount, 100))

	resource_mgr.transfer_resource(target, npc, resource_type, steal_amount)
	var crime := Crime.new(Crime.Category.THEFT, npc)
	WorldState.add_pending_crime(crime)
	return steal_amount