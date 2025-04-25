class_name ActionData
extends Node

var action: PCG.Action
var data: Dictionary = {}

func _init(_action: PCG.Action, _data := {}):
	action = _action
	data = _data