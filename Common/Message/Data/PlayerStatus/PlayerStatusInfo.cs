﻿using Lidgren.Network;
using LunaCommon.Message.Base;

namespace LunaCommon.Message.Data.PlayerStatus
{
    public class PlayerStatusInfo
    {
        public string PlayerName;
        public string VesselText;
        public string StatusText;

        public void Serialize(NetOutgoingMessage lidgrenMsg, bool dataCompressed)
        {
            lidgrenMsg.Write(PlayerName);
            lidgrenMsg.Write(VesselText);
            lidgrenMsg.Write(StatusText);
        }

        public void Deserialize(NetIncomingMessage lidgrenMsg, bool dataCompressed)
        {
            PlayerName = lidgrenMsg.ReadString();
            VesselText = lidgrenMsg.ReadString();
            StatusText = lidgrenMsg.ReadString();
        }

        public int GetByteCount(bool dataCompressed)
        {
            return PlayerName.GetByteCount() + VesselText.GetByteCount() + StatusText.GetByteCount();
        }
    }
}
