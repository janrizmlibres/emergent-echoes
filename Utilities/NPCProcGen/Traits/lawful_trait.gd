class_name LawfulTrait
extends BaseTrait

@export var investigation_duration: float = 300

var assigned_case: Crime = null:
	get:
		return assigned_case
	set(value):
		assigned_case = value
		if assigned_case != null:
			timer = investigation_duration

var timer: float = investigation_duration

func _process(delta):
	if assigned_case == null: return
	timer -= delta

func evaluation_proactive_action():
	if timer <= 0:
		resolve_case()
		return

	if assigned_case == null:
		assigned_case = WorldState.get_open_case()
	
	if assigned_case == null: return

	var target = assigned_case.get_random_participant()
	if target != null:
		add_action(Globals.Action.INTERROGATE, Globals.ResourceType.DUTY, [assigned_case, target])
		return
	
	if assigned_case.all_participants_cleared():
		add_action(Globals.Action.PURSUIT, Globals.ResourceType.DUTY, [assigned_case])

func resolve_case():
	var probability = assigned_case.get_solve_probability()
	if (randf() < probability):
		add_action(Globals.Action.PURSUIT, Globals.ResourceType.DUTY, [assigned_case])
	else:
		assigned_case.status = Crime.Status.UNSOLVED
		assigned_case = null