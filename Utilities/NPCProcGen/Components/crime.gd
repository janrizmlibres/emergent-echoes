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
		if verifiers.has(p): return false
		if falsifiers.has(p): return false
		return WorldState.actor_state[p].is_available
	)

	if not filtered_participants.is_empty():
		return filtered_participants.pick_random()
	
	return null

func all_participants_cleared():
	return verifiers.size() + falsifiers.size() == participants.size()

func get_solve_probability():
	if participants.size() > 3:
		return float(verifiers.size()) / participants.size()
	
	return 0.1 + 0.2 * verifiers.size()