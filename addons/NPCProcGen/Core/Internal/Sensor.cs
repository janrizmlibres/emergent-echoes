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

        public static void Initialize(List<ActorTag2D> actors, List<PrisonMarker2D> prisons,
            List<CropMarker2D> cropTiles)
        {
            _worldState.Prisons.AddRange(prisons);
            _worldState.CropTiles.AddRange(cropTiles);

            foreach (ActorTag2D actor in actors)
            {
                actor.Initialize(actors.Where(a => actor != a).ToList());
                _worldState.ActorState.Add(actor, new ActorState());
            }
        }

        public static void Update(double delta)
        {
            _worldState.CropTiles.ForEach(tile => tile.Update(delta));
        }

        public static int GetActorCount()
        {
            return _worldState.ActorState.Keys.ToList().Count;
        }

        public static List<ActorTag2D> GetActors()
        {
            return _worldState.ActorState.Keys.ToList();
        }

        public static void RecordCrime(Crime crime)
        {
            _worldState.Crimes.Add(crime);
        }

        public static bool HasCrime()
        {
            return _worldState.Crimes.Count > 0;
        }

        public static PrisonMarker2D GetRandomPrison()
        {
            DebugTool.Assert(_worldState.Prisons.Count > 0, "No prisons available.");
            return CommonUtils.Shuffle(_worldState.Prisons).First();
        }

        public static bool HasMatureCropTile()
        {
            return _worldState.CropTiles.Any(tile => tile.Status == CropStatus.Grown);
        }

        public static bool HasAvailableCropTile()
        {
            return _worldState.CropTiles.Any(tile => tile.Status == CropStatus.Dormant);
        }

        public CropMarker2D GetMatureCropTile()
        {
            DebugTool.Assert(_worldState.CropTiles.Count > 0, "No crop tiles available.");

            List<CropMarker2D> cropTiles = _worldState.CropTiles
                .Where(tile => tile.Status == CropStatus.Grown)
                .ToList();

            return cropTiles.OrderBy(DistanceToActor).FirstOrDefault();
        }

        public CropMarker2D GetAvailableCropTile()
        {
            DebugTool.Assert(_worldState.CropTiles.Count > 0, "No crop tiles available.");

            List<CropMarker2D> cropTiles = _worldState.CropTiles
                .Where(tile => tile.Status == CropStatus.Dormant)
                .ToList();

            return cropTiles.OrderBy(DistanceToActor).FirstOrDefault();
        }

        private float DistanceToActor(CropMarker2D tile)
        {
            return tile.GlobalPosition.DistanceTo(
                _actorCtx.ActorNode2D.GlobalPosition
            );
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
                || state == ActionState.Capture || state == ActionState.Interrogate;
        }

        public bool IsUnavailable()
        {
            return _worldState.ActorState[_actorCtx.Actor].IsUnavailable;
        }

        public void SetAvailability(bool isAvailable)
        {
            _worldState.ActorState[_actorCtx.Actor].IsUnavailable = !isAvailable;
            if (isAvailable) return;
            _worldState.Crimes.ForEach(crime => crime.Participants.Remove(_actorCtx.Actor));
        }

        public ResourceType GetPetitionResourceType()
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

        public Crime AssignCase()
        {
            Crime pendingCrime = _worldState.Crimes.Where(c => c.IsOpen()).FirstOrDefault();

            if (pendingCrime != null)
            {
                pendingCrime.Investigator = _actorCtx.GetNPCAgent2D();
                return pendingCrime;
            }

            return null;
        }
    }
}