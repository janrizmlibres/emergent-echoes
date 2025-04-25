class_name ActorData
extends Node

var relationship := 10.0:
	get:
		return relationship
	set(value):
		relationship = clamp(value, 0.0, 40.0)