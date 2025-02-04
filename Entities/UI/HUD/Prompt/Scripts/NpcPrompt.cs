using Godot;
using System;

public abstract partial class NpcPrompt : CanvasLayer
{
    protected Interactable _interactable;
    protected NpcName _npcName;
    protected NpcDescription _npcDescription;
    protected Traits _npcTraits;

    protected void DisplayDescription()
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

        if (_interactable != null)
        {
            _interactable.PrimaryInteract = new Callable(this, nameof(OnInteract));
        }
        else
        {
            GD.PrintErr("Interactable node not found!");
        }
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

    protected abstract void OnInteract();

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