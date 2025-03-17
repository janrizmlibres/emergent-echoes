class_name LawfulTrait
extends BaseTrait

@export var investigation_duration: float = 300

var assigned_case: Crime = null:
	get:
		return assigned_case
	set(value):
		assigned_case = value
		timer = investigation_duration

var timer: float = investigation_duration

func _process(delta):
	if assigned_case == null: return
	timer -= delta

func evaluation_proactive_action():
	if timer <= 0:
		print("Closed case during timeout")
		resolve_case()
		return

	if assigned_case == null:
		assigned_case = WorldState.get_open_case(actor_node as NPC)
	
	if assigned_case == null:
		print("No open cases")
		return

	print("A case has been assigned to ", actor_node.name)
	var target = assigned_case.get_random_participant()
	if target != null:
		add_action(Globals.Action.INTERROGATE, Globals.ResourceType.DUTY, {
			"case": assigned_case,
			"target": target,
			"assess_completed": false
		})
		return
	
	if assigned_case.all_participants_cleared():
		resolve_case()

func resolve_case():
	if assigned_case.close(10): return

	add_action(Globals.Action.PURSUIT, Globals.ResourceType.DUTY, {
		"case": assigned_case,
		"target": assigned_case.criminal,
		"assess_completed": false
	})
