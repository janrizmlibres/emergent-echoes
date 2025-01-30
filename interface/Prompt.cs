using Godot;
using System;

public partial class Prompt : CanvasLayer
{
    private Interactable _interactable;
    private NpcName _npcName;
    private NpcDescription _npcDescription;
    private Traits _npcTraits;

    private void DisplayDescription()
    {
        this.Visible = !this.Visible;

        // Pause or unpause the game
        Engine.TimeScale = this.Visible ? 0 : 1;
    }

    public override void _Ready()
    {
        _interactable = GetParent().GetNode<Interactable>("Interactable");
        _npcName = FindNpcNameNode(GetParent());
        _npcDescription = FindNpcDescriptionNode(GetParent());
        _npcTraits = FindNpcTraitsNode(GetParent());

        _interactable.Interact = new Callable(this, nameof(OnInteract));
    }

    private NpcName FindNpcNameNode(Node parent)
    {
        foreach (Node child in parent.GetChildren())
        {
            if (child is NpcName npcName)
            {
                return npcName;
            }
            else
            {
                NpcName found = FindNpcNameNode(child);
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }

    private NpcDescription FindNpcDescriptionNode(Node parent)
    {
        foreach (Node child in parent.GetChildren())
        {
            if (child is NpcDescription npcDescription)
            {
                return npcDescription;
            }
            else
            {
                NpcDescription found = FindNpcDescriptionNode(child);
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }

    private Traits FindNpcTraitsNode(Node parent)
    {
        foreach (Node child in parent.GetChildren())
        {
            if (child is Traits npcTraits)
            {
                return npcTraits;
            }
            else
            {
                Traits found = FindNpcTraitsNode(child);
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }

    private void OnInteract()
    {
        UpdateNpcName("Silas");
        UpdateNpcDescription("This is Silas, a villager. He is always ready to steal, so be careful. He is a known theif.");
        UpdateNpcTraits("Brave, Loyal, Strong");
        DisplayDescription();
    }

    public void UpdateNpcName(string name)
    {
        if (_npcName != null)
        {
            _npcName.SetNpcName(name);
        }
    }

    public void UpdateNpcDescription(string description)
    {
        if (_npcDescription != null)
        {
            _npcDescription.SetNpcDescription(description);
        }
    }

    public void UpdateNpcTraits(string traits)
    {
        if (_npcTraits != null)
        {
            _npcTraits.SetNpcTraits(traits);
        }
    }
}