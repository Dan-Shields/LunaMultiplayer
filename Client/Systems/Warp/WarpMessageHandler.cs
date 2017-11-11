﻿using LunaClient.Base;
using LunaClient.Base.Interface;
using LunaClient.Systems.SettingsSys;
using LunaCommon.Enums;
using LunaCommon.Message.Data.Warp;
using LunaCommon.Message.Interface;
using LunaCommon.Message.Types;
using System.Collections.Concurrent;

namespace LunaClient.Systems.Warp
{
    public class WarpMessageHandler : SubSystem<WarpSystem>, IMessageHandler
    {
        public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

        public void HandleMessage(IServerMessageBase msg)
        {
            if (!(msg.Data is WarpBaseMsgData msgData)) return;

            switch (msgData.WarpMessageType)
            {
                case WarpMessageType.SubspacesReply:
                    {
                        var data = (WarpSubspacesReplyMsgData)msgData;
                        for (var i = 0; i < data.SubspaceKey.Length; i++)
                        {
                            AddSubspace(data.SubspaceKey[i], data.SubspaceTime[i]);
                        }
                        foreach (var ps in data.Players)
                        {
                            if (System.ClientSubspaceList.ContainsKey(ps.Value))
                            {
                                System.ClientSubspaceList[ps.Value] = ps.Key;
                            }
                            else
                            {
                                System.ClientSubspaceList.TryAdd(ps.Value, ps.Key);
                            }
                        }

                        AddSubspace(-1, 0);//Add warping subspace

                        MainSystem.NetworkState = ClientState.WarpsubspacesSynced;
                    }
                    break;
                case WarpMessageType.NewSubspace:
                    {
                        var data = (WarpNewSubspaceMsgData)msgData;
                        AddSubspace(data.SubspaceKey, data.ServerTimeDifference);
                        if (data.PlayerCreator == SettingsSystem.CurrentSettings.PlayerName)
                        {
                            //It's our subspace that we just created so set it as ours
                            System.WaitingSubspaceIdFromServer = false;
                            System.SkipSubspaceProcess = true;
                            System.CurrentSubspace = data.SubspaceKey;
                        }
                    }
                    break;
                case WarpMessageType.ChangeSubspace:
                    {
                        var data = (WarpChangeSubspaceMsgData)msgData;
                        System.ClientSubspaceList[data.PlayerName] = data.Subspace;
                    }
                    break;
                default:
                    {
                        LunaLog.LogError($"[LMP]: Unhandled WARP_MESSAGE type: {msgData.WarpMessageType}");
                        break;
                    }
            }
        }

        private static void AddSubspace(int subspaceId, double subspaceTime)
        {
            if (!System.Subspaces.ContainsKey(subspaceId))
                System.Subspaces.TryAdd(subspaceId, subspaceTime);
        }
    }
}