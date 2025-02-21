using System;
using System.Collections.Generic;
using System.Linq;
using NPCProcGen.Autoloads;
using NPCProcGen.Core.Components;
using NPCProcGen.Core.Components.Enums;
using NPCProcGen.Core.Helpers;

namespace NPCProcGen.Core.Internal
{
    public class Sensor
    {
        private static readonly WorldState _worldState = new();

        private readonly ActorContext _actorCtx;

        public Sensor(ActorContext context)
        {
            _actorCtx = context;
        }

        public static void Initialize(List<ActorTag2D> actors, List<PrisonArea2D> prisons)
        {
            _worldState.Actors.AddRange(actors);
            _worldState.Prisons.AddRange(prisons);

            foreach (ActorTag2D actor in actors)
            {
                actor.Initialize(actors.Where(a => actor != a).ToList());
                _worldState.ActorState.Add(actor, new ActorState());
            }
        }

        public List<ActorTag2D> GetActors()
        {
            return _worldState.Actors.ToList();
        }

        public Tuple<ActionType, ActionState> GetTaskRecord()
        {
            return _worldState.ActorState[_actorCtx.Actor].CurrentTask ?? null;
        }

        public void SetTaskRecord(ActionType actionType, ActionState actionState)
        {
            ActorState actorState = _worldState.ActorState[_actorCtx.Actor];
            actorState.CurrentTask = new Tuple<ActionType, ActionState>(
                actionType, actionState
            );
        }

        public void ClearTaskRecord()
        {
            _worldState.ActorState[_actorCtx.Actor].CurrentTask = null;
        }

        public bool IsBusy()
        {
            Tuple<ActionType, ActionState> action = GetTaskRecord();

            if (action == null) return false;

            ActionState state = action.Item2;

            return state == ActionState.Talk || state == ActionState.Petition
                || state == ActionState.Interact || state == ActionState.Flee
                || state == ActionState.Capture;
        }

        public bool IsUnavailable()
        {
            return _worldState.ActorState[_actorCtx.Actor].IsUnavailable;
        }

        public ResourceType? GetPetitionResourceType()
        {
            return _worldState.ActorState[_actorCtx.Actor].CurrentPetitionResourceType;
        }

        public void SetPetitionResourceType(ResourceType type)
        {
            _worldState.ActorState[_actorCtx.Actor].CurrentPetitionResourceType = type;
        }

        public void ClearPetitionResourceType()
        {
            SetPetitionResourceType(ResourceType.None);
        }

        public void RecordCrime(Crime crime)
        {
            _worldState.RecordedCrimes.Enqueue(crime);
        }

        public Crime InvestigateCrime()
        {
            if (_worldState.RecordedCrimes.TryDequeue(out Crime crime))
            {
                NPCAgent2D investigator = _actorCtx.GetNPCAgent2D();
                crime.Investigator = investigator;
                _worldState.Investigations.Add(investigator, crime);
                return crime;
            }
            return null;
        }

        public void CloseInvestigation()
        {
            Crime crime = RemoveInvestigation();
            _worldState.UnsolvedCrimes.Add(crime);
        }

        public void SolveInvestigation()
        {
            Crime crime = RemoveInvestigation();
            _worldState.SolvedCrimes.Add(crime);
        }

        public PrisonArea2D GetRandomPrison()
        {
            return CommonUtils.Shuffle(_worldState.Prisons).First();
        }

        private Crime RemoveInvestigation()
        {
            NPCAgent2D investigator = _actorCtx.GetNPCAgent2D();
            Crime crime = _worldState.Investigations[investigator];
            _worldState.Investigations.Remove(investigator);
            return crime;
        }
    }
}