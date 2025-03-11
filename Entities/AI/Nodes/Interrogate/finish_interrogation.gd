@tool
extends ActionLeaf

func tick(actor: Node, blackboard: Blackboard) -> int:
  var npc = actor as NPC
  var crime: Crime = blackboard.get_value("crime")
  var target = blackboard.get_value("target")

  var relationship = target.memorizer.get_actor_relationship(npc)
  var probability = get_interrogation_probability(relationship)

  if randf() <= probability:
    crime.verifiers.append(target)
  else:
    crime.falsifiers.append(target)

  if crime.all_participants_cleared():
    var solve_probability = crime.get_solve_probability()
    if randf() > solve_probability:
      crime.status = Crime.Status.UNSOLVED
      npc.lawful_trait.assigned_case = null

  npc.executor.end_action()
  return SUCCESS

func get_interrogation_probability(relationship_level: float):
  const THRESHOLDS = {
    -26: 0.30,
    -16: 0.50,
    -6: 0.70,
    4: 0.90,
    14: 0.92,
    24: 0.95
  }

  for threshold in THRESHOLDS.keys():
    if relationship_level <= threshold:
      return THRESHOLDS[threshold]

  return 1.00