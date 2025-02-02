using Godot;
using System;

public partial class SilasPrompt : NpcPrompt
{
    protected override void OnInteract()
    {
        UpdateNpcName("Silas");
        UpdateNpcDescription("This is Silas, a villager. He is always ready to steal, so be careful. He is a known thief.");
        UpdateNpcTraits("Thief, Survival");
        DisplayDescription();
    }
}