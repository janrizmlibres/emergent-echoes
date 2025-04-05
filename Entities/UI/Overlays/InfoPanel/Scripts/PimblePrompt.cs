using Godot;
using System;

public partial class PimblePrompt : NpcPrompt
{
    protected override void OnInteract()
    {
        UpdateNpcName("Pimble");
        UpdateNpcDescription("This is Pimble, a curious adventurer.");
        UpdateNpcTraits("Curious, Brave, Energetic");
        DisplayDescription();
    }
}