using Godot;
using System;

public partial class ConfirmationLabel : Label
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // Method to update the text with ResourceType and ActorName
    public void UpdateText(string resourceType, string actorName)
    {
        this.Text = $"ARE YOU SURE YOU WANT TO GIVE {resourceType} TO {actorName}?";
    }
}