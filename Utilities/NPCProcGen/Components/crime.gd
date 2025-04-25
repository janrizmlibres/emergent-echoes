class_name Crime

enum Category {
	THEFT,
	MURDER,
	VANDALISM
}

enum Outcome {
	PENDING,
	SOLVED,
	UNSOLVED
}

var category: Category
var criminal: Actor

var _participants: Dictionary[Actor, bool] = {}
var _verifiers: Dictionary[Actor, bool] = {}
var _falsifiers: Dictionary[Actor, bool] = {}
var _outcome := Outcome.PENDING

func _init(_category: Category, _criminal: Actor):
	category = _category
	criminal = _criminal

func record_participant(actor: Actor):
	_participants[actor] = true

func mark_verifier(actor: Actor):
	assert(_participants.has(actor), "Actor is not a participant")
	assert(not _falsifiers.has(actor), "Actor already in falsifiers")
	_verifiers[actor] = true

func mark_falsifier(actor: Actor):
	assert(_participants.has(actor), "Actor is not a participant")
	assert(not _verifiers.has(actor), "Actor already in verifiers")
	_falsifiers[actor] = true

func cleanse_actor(actor: Actor):
	if actor == criminal:
		criminal = null

	_participants.erase(actor)
	_verifiers.erase(actor)
	_falsifiers.erase(actor)

func select_participant():
	var filtered_participants = _participants.keys().filter(func(p):
		if p == null: return false
		if _verifiers.has(p): return false
		if _falsifiers.has(p): return false
		return true
	)

	return filtered_participants.pick_random() if not filtered_participants.is_empty() else null

func all_participants_cleared():
	var cleared_count = _verifiers.size() + _falsifiers.size()
	return cleared_count == _participants.size()

func close_case():
	assert(not is_closed(), "Crime is already closed")

	var probability: float
	if _participants.size() > 3:
		probability = _verifiers.size() as float / _participants.size()
	else:
		probability = 0.1 + 0.3 * _verifiers.size()
	
	_outcome = Outcome.SOLVED if randf() < probability else Outcome.UNSOLVED

func is_closed() -> bool:
	return _outcome != Outcome.PENDING

func is_solved() -> bool:
	return _outcome == Outcome.SOLVED

func reset() -> void:
	_verifiers.clear()
	_falsifiers.clear()
	_outcome = Outcome.PENDING
