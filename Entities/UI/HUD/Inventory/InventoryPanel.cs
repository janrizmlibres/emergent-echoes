using EmergentEchoes.Entities.UI.Inventory.Items;
using EmergentEchoes.Entities.UI.Inventory.Slot;
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmergentEchoes.Entities.UI.HUD.Inventory
{
	public partial class InventoryPanel : Panel
	{
		private readonly List<InventorySlot> _slots = new();
		private int _selectedSlot;

		public override void _Ready()
		{
			Array<Node> slots = GetNode<HBoxContainer>("SlotContainer").GetChildren();

			foreach (Node child in slots)
			{
				if (child is not InventorySlot inSlot)
				{
					GD.PrintErr($"Child node {child.Name} is not an InventorySlot node, it is of type {child.GetType()}");
					throw new InvalidCastException();
				}

				_slots.Add(inSlot);

				// if (child.HasMethod("SetItem"))
				// {
				// 	child.SetItem(null);
				// 	child.InventoryWindow = this;
				// }

				// child.SetItem(null);
				// child.InventoryWindow = this;
			}

			_selectedSlot = 0;
		}

		public override void _Process(double delta)
		{
			if (Input.IsActionJustPressed("inventory"))
			{
				ToggleWindow(!Visible);
			}
		}

		public override void _Input(InputEvent @event)
		{
			if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
			{
				switch (mouseEvent.ButtonIndex)
				{
					case MouseButton.WheelUp:
						_selectedSlot = (_selectedSlot - 1 + _slots.Count) % _slots.Count;
						break;
					case MouseButton.WheelDown:
						_selectedSlot = (_selectedSlot + 1) % _slots.Count;
						break;
				}
			}

			// if (@event is InputEventKey keyEvent && keyEvent.Pressed)
			// {
			// 	int keyNumber = keyEvent.Keycode - (int)KeyList.Key0;
			// 	if (keyNumber >= 0 && keyNumber < _slots.Count)
			// 	{
			// 		_selectedSlot = keyNumber;
			// 	}
			// }
		}

		public void ToggleWindow(bool isOpen)
		{
			Visible = isOpen;
			Input.MouseMode = isOpen ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
		}

		// public void OnGivePlayerItem(TempItem item, int amount)
		// {

		// }

		public void AddItem(BaseItem item)
		{
			InventorySlot slot = GetSlotToAdd(item);

			if (slot == null) return;

			if (slot.Item == null)
			{
				slot.SetItem(item);
			}
			else if (slot.Item == item)
			{
				slot.AddItem();
			}
		}

		public void RemoveItem(BaseItem item)
		{
			InventorySlot slot = GetSlotToRemove(item);

			if (slot == null || slot.Item == item) return;

			slot.RemoveItem();
		}

		public InventorySlot GetSlotToAdd(BaseItem item)
		{
			foreach (InventorySlot slot in _slots)
			{
				if (slot.Item == item && slot.Quantity < item.MaxStackSize)
				{
					return slot;
				}
			}

			foreach (InventorySlot slot in _slots)
			{
				if (slot.Item == null)
				{
					return slot;
				}
			}

			return null;
		}

		public InventorySlot GetSlotToRemove(BaseItem item)
		{
			foreach (InventorySlot slot in _slots)
			{
				if (slot.Item == item)
				{
					return slot;
				}
			}

			return null;
		}

		public int GetNumberOfItem(BaseItem item)
		{
			int total = 0;

			foreach (InventorySlot slot in _slots)
			{
				if (slot.Item == item)
				{
					total += slot.Quantity;
				}
			}

			return total;
		}
	}
}