using Godot;

namespace EmergentEchoes.Entities.UI.Inventory.Items
{
    public abstract class Consumable : BaseItem
    {
        public Consumable(string name, Texture2D icon, int maxStackSize)
            : base(name, icon, maxStackSize)
        {
        }

        public abstract void Consume();
    }
}