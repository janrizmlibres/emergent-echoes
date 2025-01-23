using Godot;

namespace EmergentEchoes.Entities.UI.Inventory.Items
{
    public abstract class BaseItem
    {
        public string Name { get; private set; }
        public Texture2D Icon { get; private set; }
        public int MaxStackSize { get; private set; }

        public BaseItem(string name, Texture2D icon, int maxStackSize)
        {
            Name = name;
            Icon = icon;
            MaxStackSize = maxStackSize;
        }
    }
}