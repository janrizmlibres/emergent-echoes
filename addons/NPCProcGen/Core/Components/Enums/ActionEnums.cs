namespace NPCProcGen.Core.Components.Enums
{
    /// <summary>
    /// Represents the types of actions that can be performed.
    /// </summary>
    public enum ActionType
    {
        Theft,
        Eat,
        Petition,
        Socialize,
        Interact,
        Investigate,
        Pursuit,
        CloseCase,
    }

    public enum ActionState
    {
        Move,
        Engage,
        Seek,
        Search,
        Sneak,
        Wait,
        Wander,
        Steal,
        Flee,
        Eat,
        Petition,
        Talk,
        Interact,
        Research,
        Interrogate,
        Capture
    }

    public enum InteractState
    {
        Petition = ActionState.Petition,
        Talk = ActionState.Talk,
    }
}