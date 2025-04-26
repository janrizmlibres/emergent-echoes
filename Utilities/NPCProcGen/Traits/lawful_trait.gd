class_name LawfulTrait
extends BaseTrait

var investigation_timer := Timer.new()

var current_case: Crime = null:
	get:
		return current_case
	set(value):
		current_case = value

		if value != null:
			investigation_timer.start()
		else:
			investigation_timer.stop()

func _ready():
	investigation_timer.wait_time = 300
	investigation_timer.one_shot = true
	investigation_timer.timeout.connect(_on_investigation_timeout)
	add_child(investigation_timer)

func evaluation_proactive_action():
	if not has_case():
		return
	
	if current_case.all_participants_cleared():
		if not current_case.is_closed():
			current_case.close_case()
			investigation_timer.stop()
		
		if current_case.is_solved():
			add_action(PCG.Action.PURSUIT, PCG.ResourceType.DUTY, {
				"target": current_case.criminal,
				"is_reactive": false
			})
		else:
			add_action(PCG.Action.ASSESS, PCG.ResourceType.DUTY, {
				"target": current_case.criminal
			})
		return
		
	var target = current_case.select_participant()
	if target != null:
		add_action(PCG.Action.INTERROGATE, PCG.ResourceType.DUTY, {
			"case": current_case,
			"target": target
		})
		return
	
func has_case() -> bool:
	if current_case == null:
		current_case = WorldState.get_pending_crime()
	
	return true if current_case != null else false

func _on_investigation_timeout():
	current_case.close_case()
