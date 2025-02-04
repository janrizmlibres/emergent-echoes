using Godot;
using System;

public partial class GarrethPrompt : NpcPrompt
{
    protected override void OnInteract()
    {
        UpdateNpcName("Garreth");
        UpdateNpcDescription("This is Garreth, a wise sage.");
        UpdateNpcTraits("Wise, Calm, Knowledgeable");
        DisplayDescription();
    }
}