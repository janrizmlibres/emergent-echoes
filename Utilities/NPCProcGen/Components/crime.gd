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
var status := Status.PENDING
var category: Category
var criminal: Actor

var _participants: Dictionary[Actor, bool] = {}
var _verifiers: Dictionary[Actor, bool] = {}
var _falsifiers: Dictionary[Actor, bool] = {}

func _init(_category: Category, _criminal: Actor):
	category = _category
	criminal = _criminal

func is_open():
	return investigator == null and status == Status.PENDING

func record_participant(actor: Actor):
	_participants[actor] = true

func cleanse_actor(actor: Actor):
	# if actor is the investigator or criminal, do something
	_participants.erase(actor)
	_verifiers.erase(actor)
	_falsifiers.erase(actor)

func get_random_participant():
	var filtered_participants = _participants.keys().filter(func(p):
		if not is_instance_valid(p): return false
		if _verifiers.has(p): return false
		if _falsifiers.has(p): return false
		return true
	)

	return filtered_participants.pick_random()

func all_participants_cleared():
	var cleared = _verifiers.size() + _falsifiers.size()
	assert(cleared <= _participants.size(), "Cleared more _participants then actual count")
	return _verifiers.size() + _falsifiers.size() == _participants.size()

func close(duty_increase: float) -> bool:
	assert(investigator != null, "Cannot close case without investigator")

	if randf() >= get_solve_probability():
		status = Status.UNSOLVED
		complete_investigation(duty_increase)
		return true

	if not is_instance_valid(criminal) or WorldState.is_captured(criminal):
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
	if _participants.size() > 3:
		return float(_verifiers.size()) / _participants.size()
	
	return 0.1 + 0.2 * _verifiers.size()
