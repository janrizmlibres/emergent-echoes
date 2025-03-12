using Godot;
using System;

public partial class HeartBar : Control
{
    [Export] public int MaxHealth = 3; // Only 3 hearts
    [Export] public int CurrentHealth = 3; // Start full

    [Export] public Texture2D FullHeart;
    [Export] public Texture2D EmptyHeart;

    private HBoxContainer heartContainer;

    public override void _Ready()
    {
        heartContainer = GetNode<HBoxContainer>("HBoxContainer");
        UpdateHearts();
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < MaxHealth; i++)
        {
            TextureRect heart = (TextureRect)heartContainer.GetChild(i);
            heart.Texture = (i < CurrentHealth) ? FullHeart : EmptyHeart;
        }
    }

    public void ChangeHealth(int value)
    {
        CurrentHealth = Math.Clamp(CurrentHealth + value, 0, MaxHealth);
        UpdateHearts();
    }
}
