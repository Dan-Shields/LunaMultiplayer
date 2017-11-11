﻿using LunaClient.Base;
using LunaClient.Base.Interface;
using LunaClient.Systems.SettingsSys;
using LunaCommon.Enums;
using LunaCommon.Message.Data.Chat;
using LunaCommon.Message.Interface;
using LunaCommon.Message.Types;
using System.Collections.Concurrent;

namespace LunaClient.Systems.Chat
{
    public class ChatMessageHandler : SubSystem<ChatSystem>, IMessageHandler
    {
        public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

        public void HandleMessage(IServerMessageBase msg)
        {
            if (!(msg.Data is ChatBaseMsgData msgData)) return;

            switch (msgData.ChatMessageType)
            {
                case ChatMessageType.ListReply:
                    {
                        var data = (ChatListReplyMsgData)msgData;
                        foreach (var keyVal in data.PlayerChannels)
                            foreach (var channelName in keyVal.Value)
                                System.Queuer.QueueChatJoin(keyVal.Key, channelName);

                        MainSystem.NetworkState = ClientState.ChatSynced;
                    }
                    break;
                case ChatMessageType.Join:
                    {
                        var data = (ChatJoinMsgData)msgData;
                        System.Queuer.QueueChatJoin(data.From, data.Channel);
                    }
                    break;
                case ChatMessageType.Leave:
                    {
                        var data = (ChatLeaveMsgData)msgData;
                        System.Queuer.QueueChatLeave(data.From, data.Channel);
                    }
                    break;
                case ChatMessageType.ChannelMessage:
                    {
                        var data = (ChatChannelMsgData)msgData;
                        System.Queuer.QueueChannelMessage(data.From, data.Channel, data.Text);
                    }
                    break;
                case ChatMessageType.PrivateMessage:
                    {
                        var data = (ChatPrivateMsgData)msgData;
                        if (data.To == SettingsSystem.CurrentSettings.PlayerName ||
                            data.From == SettingsSystem.CurrentSettings.PlayerName)
                            System.Queuer.QueuePrivateMessage(data.From, data.To, data.Text);
                    }
                    break;
                case ChatMessageType.ConsoleMessage:
                    {
                        var data = (ChatConsoleMsgData)msgData;
                        System.Queuer.QueueSystemMessage(data.Message);
                    }
                    break;
            }
        }
    }
}