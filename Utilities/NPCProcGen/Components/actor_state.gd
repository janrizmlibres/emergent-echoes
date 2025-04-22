class_name ActorState
extends Node

enum State {
	FREE,
	OCCUPIED,
	INDISPOSED,
	CAPTURED
}

var current_action := PCG.Action.WANDER
var status := State.FREE