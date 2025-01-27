namespace NPCProcGen.Core.Components.Enums
{
    /// <summary>
    /// Represents the different states an action can be in.
    /// </summary>
    public enum ActionState
    {
        /// <summary>
        /// The entity is moving.
        /// </summary>
        Move,
        /// <summary>
        /// The entity is wandering.
        /// </summary>
        Wander,
        /// <summary>
        /// The entity is stealing.
        /// </summary>
        Steal,
        /// <summary>
        /// The entity is fleeing.
        /// </summary>
        Flee,
        Eat,
        Petition,
        Seek,
        Talk,
    }
}