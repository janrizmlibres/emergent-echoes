using Godot;

[GlobalClass]
public partial class ConfirmationLabel : Label
{
    public void UpdateText(string quantity, string resourceType, string actorName)
    {
        this.Text = $"ARE YOU SURE YOU WANT TO GIVE {quantity} {resourceType} TO {actorName}?";
    }
}