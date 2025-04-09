class_name BaseCamera
extends Camera2D

func _ready():
	var top_left = $Limits/TopLeft
	var bottom_right = $Limits/BottomRight

	limit_left = top_left.global_position.x
	limit_top = top_left.global_position.y
	limit_right = bottom_right.global_position.x
	limit_bottom = bottom_right.global_position.y
