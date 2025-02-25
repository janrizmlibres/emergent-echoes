namespace NPCProcGen.Core.Components.Enums
{
    public enum ActionType
    {
        Theft,
        Eat,
        Petition,
        Socialize,
        Interact,
        Assess,
        Interrogate,
        Pursuit
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
        Assess,
        Interrogate,
        Capture
    }

    public enum InteractState
    {
        Petition = ActionState.Petition,
        Talk = ActionState.Talk,
        Interrogate = ActionState.Interrogate,
        Capture = ActionState.Capture
    }
}