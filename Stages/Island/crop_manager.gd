extends Node2D

@onready var crops = $"."
@onready var crop_stage = 0
@onready var grow_timer = $"../../GrowTimer"

func _on_grow_timer_timeout() -> void:
	var crops_to_process = crops.get_children()
	var crop_index = 0
	
	while crop_index < crops_to_process.size():
		crops_to_process[crop_index].frame = crop_stage
		crop_index += 1
		
	if crop_stage >= 3:
		GameManager.crop_matured = true
		crop_index = 0
		crop_stage = 0
		return
		
	crop_stage += 1
	grow_timer.start()
	pass # Replace with function body.
