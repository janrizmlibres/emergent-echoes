using System;
using Godot;

namespace EmergentEchoes.Entities.UI.Inventory.Items
{
    public class FoodItem : Consumable
    {
        public FoodItem(string name, Texture2D icon, int maxStackSize)
            : base(name, icon, maxStackSize)
        {
        }

        public override void Consume()
        {
        }
    }
}