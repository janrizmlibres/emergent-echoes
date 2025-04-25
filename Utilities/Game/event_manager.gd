extends Node

signal info_dialog_requested(actor: Actor)
signal show_player_petition_hud(target: Actor)
signal show_npc_petition_hud(npc_petitioner: NPC, resource_type: PCG.ResourceType, quantity: int)

func emit_info_dialog_requested(actor: Actor) -> void:
	info_dialog_requested.emit(actor)

func emit_show_player_petition_hud(target) -> void:
	show_player_petition_hud.emit(target)

func emit_show_npc_petition_hud(npc_petitioner: NPC, resource_type, quantity: int) -> void:
	show_npc_petition_hud.emit(npc_petitioner, resource_type, quantity)