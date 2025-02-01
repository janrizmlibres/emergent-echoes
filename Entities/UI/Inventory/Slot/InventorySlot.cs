using EmergentEchoes.Entities.UI.HUD.Inventory;
using EmergentEchoes.Entities.UI.Inventory.Items;
using Godot;
using System;

namespace EmergentEchoes.Entities.UI.Inventory.Slot
{
	public partial class InventorySlot : Button
	{
		public InventoryPanel InventoryWindow { get; set; }
		public BaseItem Item { get; private set; }
		public int Quantity { get; private set; } = 0;

		private TextureRect _icon;
		private Label _quantityLabel;

		public override void _Ready()
		{
			_icon = GetNode<TextureRect>("Icon");
			_quantityLabel = GetNode<Label>("QuantityText");
		}

		public void SetItem(BaseItem newItem)
		{
			Item = newItem;
			Quantity = 1;

			if (Item == null)
			{
				_icon.Visible = false;
			}
			else
			{
				_icon.Visible = true;
				_icon.Texture = Item.Icon;
			}

			UpdateQuantityText();
		}

		public void AddItem()
		{
			Quantity += 1;
			UpdateQuantityText();
		}

		public void RemoveItem()
		{
			Quantity -= 1;
			UpdateQuantityText();

			if (Quantity <= 0)
			{
				SetItem(null);
			}
		}

		public void UpdateQuantityText()
		{
			if (Quantity > 1)
			{
				_quantityLabel.Text = Quantity.ToString();
			}
			else
			{
				_quantityLabel.Text = "";
			}
		}
	}
}