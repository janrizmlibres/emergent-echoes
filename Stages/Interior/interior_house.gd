extends Node2D

@onready var player_anim_tree: AnimationTree = $Player/AnimationTree
@onready var toblin_anim_tree: AnimationTree = $Toblin/AnimationTree
@onready var animation_player: AnimationPlayer = $AnimationPlayer

var camera_node: NodePath = "../../Camera2D"

func _ready():
	Dialogic.signal_event.connect(on_dialogic_event)
	$Toblin/RemoteTransform2D.remote_path = camera_node
	start_cutscene()
	
func start_cutscene() -> void:
	player_anim_tree.active = false
	toblin_anim_tree.active = false

func end_cutscene(player_dir: float, toblin_dir: float) -> void:
	player_anim_tree.set("parameters/Idle/blend_position", player_dir)
	toblin_anim_tree.set("parameters/Idle/blend_position", toblin_dir)
	player_anim_tree.active = true
	toblin_anim_tree.active = true

func start_intro() -> void:
	Dialogic.start("opening")

func start_outro() -> void:
	Dialogic.start("opening", "outro")

func activate_player() -> void:
	var player_script = load("res://Entities/Actors/Player/player.gd")
	$Player.set_script(player_script)
	$Player/RemoteTransform2D.remote_path = camera_node

func on_dialogic_event(event: String) -> void:
	match event:
		"give":
			animation_player.play("give_object")
		"success_exit":
			animation_player.play("success_exit")
		"fail_exit":
			animation_player.play("fail_exit")

func _on_exit_body_entered(body: Node2D):
	if body.name == "Player":
		get_tree().change_scene_to_file("res://Stages/Island/world.tscn")
