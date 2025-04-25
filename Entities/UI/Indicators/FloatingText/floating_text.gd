extends HBoxContainer

var normal_label = preload("res://Entities/UI/Indicators/FloatingText/Data/normal_label.tres")
var negative_label = preload("res://Entities/UI/Indicators/FloatingText/Data/negative_label.tres")

var texture_names = {
  PCG.ResourceType.MONEY: "money",
  PCG.ResourceType.FOOD: "food",
  PCG.ResourceType.SATIATION: "satiation",
  PCG.ResourceType.COMPANIONSHIP: "companionship",
  PCG.ResourceType.DUTY: "duty"
}

@onready var texture_rect: TextureRect = $TextureRect
@onready var label: Label = $Label

func show_value(resource_type, value, is_normal, duration, spread, travel) -> void:
  var texture_name = texture_names[resource_type]
  var texture = load("res://Entities/UI/Indicators/FloatingText/Art/" + texture_name + ".png")
  
  texture_rect.texture = texture
  label.text = value
  label.label_settings = normal_label if is_normal else negative_label

  var movement = travel.rotated(randf_range((-spread) / 2, spread / 2))
  pivot_offset = size / 2

  var tween = create_tween().set_trans(Tween.TRANS_LINEAR).set_ease(Tween.EASE_IN_OUT).set_parallel()
  tween.tween_property(self, "position", position + movement, duration)

  await tween.finished
  queue_free()
