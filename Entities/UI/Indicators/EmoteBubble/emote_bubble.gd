class_name EmoteBubble
extends Node2D

@export var interval_range: Vector2 = Vector2(4, 8)

var animations = {
	Globals.Emote.ELLIPSIS: "ellipsis",
	Globals.Emote.EXCLAMATION: "exclamation",
	Globals.Emote.HUM: "hum"
}

@onready var animation_player: AnimationPlayer = $AnimationPlayer
@onready var timer: Timer = $Timer

func activate():
	timer.start(randf_range(interval_range.x, interval_range.y))

func deactivate():
	timer.stop()

func show_random_emote():
	var emotes = Globals.Emote.values()
	emotes.shuffle()
	show_emote_bubble(emotes[0])
	
func show_emote_bubble(emote: Globals.Emote):
	animation_player.play(animations[emote])

func _on_timer_timeout():
	show_random_emote()