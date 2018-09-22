﻿using LmpClient.Base;
using LmpCommon.Message.Data.Vessel;

namespace LmpClient.Systems.VesselPartSyncCallSys
{
    public class VesselPartSyncCallQueue : CachedConcurrentQueue<VesselPartSyncCall, VesselPartSyncCallMsgData>
    {
        protected override void AssignFromMessage(VesselPartSyncCall value, VesselPartSyncCallMsgData msgData)
        {
            value.GameTime = msgData.GameTime;
            value.VesselId = msgData.VesselId;
            value.VesselPersistentId = msgData.VesselPersistentId;

            value.PartFlightId = msgData.PartFlightId;
            value.PartPersistentId = msgData.PartPersistentId;

            value.ModuleName = msgData.ModuleName.Clone() as string;
            value.MethodName = msgData.MethodName.Clone() as string;
        }
    }
}