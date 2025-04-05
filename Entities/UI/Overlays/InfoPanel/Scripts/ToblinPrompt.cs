using Godot;
using System;

public partial class ToblinPrompt : NpcPrompt
{
    protected override void OnInteract()
    {
        UpdateNpcName("Toblin");
        UpdateNpcDescription("This is Toblin, a skilled blacksmith.");
        UpdateNpcTraits("Skilled, Strong, Diligent");
        DisplayDescription();
    }
}