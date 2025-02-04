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
    }

    public enum ActionState
    {
        Move,
        Wander,
        Steal,
        Flee,
        Eat,
        Petition,
        Seek,
        Talk,
    }

    public enum InteractState
    {
        Petition = ActionState.Petition,
        Talk = ActionState.Talk,
    }
}