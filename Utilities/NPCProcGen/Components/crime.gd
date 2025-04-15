class_name Crime

enum Category {
	THEFT,
	MURDER,
	VANDALISM
}

enum Status {
	PENDING,
	UNSOLVED,
	SOLVED
}

var investigator: NPC = null
var status: Status = Status.PENDING
var category: Category
var criminal: Actor
var participants: Array[Actor]

var verifiers: Array[Actor] = []
var falsifiers: Array[Actor] = []

func _init(_category: Category, _criminal: Actor, _participants):
	category = _category
	criminal = _criminal
	participants = _participants

func is_open():
	return investigator == null and status == Status.PENDING

func get_random_participant():
	var filtered_participants = participants.filter(func(p):
		if not is_instance_valid(p): return false
		if verifiers.has(p): return false
		if falsifiers.has(p): return false
		return true
	)

	if not filtered_participants.is_empty():
		return filtered_participants.pick_random()
	
	return null

func all_participants_cleared():
	var cleared = verifiers.size() + falsifiers.size()
	assert(cleared <= participants.size(), "Cleared more participants then actual count")
	return verifiers.size() + falsifiers.size() == participants.size()

func close(duty_increase: float) -> bool:
	assert(investigator != null, "Cannot close case without investigator")

	if randf() >= get_solve_probability():
		Logger.info(investigator.name + " failed to solve case")
		status = Status.UNSOLVED
		complete_investigation(duty_increase)
		return true

	if not is_instance_valid(criminal) or WorldState.is_captured(criminal):
		Logger.info(investigator.name + " successfully solved case")
		status = Status.SOLVED
		complete_investigation(duty_increase)
		return true
	
	return false

func complete_investigation(duty_increase: float):
	investigator.lawful_trait.assigned_case = null
	WorldState.resource_manager.modify_resource(
		investigator,
		PCG.ResourceType.DUTY,
		duty_increase
	)
	investigator.float_text_controller.show_float_text(
		PCG.ResourceType.DUTY,
		str(duty_increase),
		true
	)
	print("Open cases: " + str(WorldState._crimes.filter(func(x): return x.is_open()).size()))

func get_solve_probability():
	if participants.size() > 3:
		return float(verifiers.size()) / participants.size()
	
	return 0.1 + 0.2 * verifiers.size()
