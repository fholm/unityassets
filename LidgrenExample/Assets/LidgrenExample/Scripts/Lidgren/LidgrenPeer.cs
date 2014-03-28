using System;
using System.Collections.Generic;
using Lidgren.Network;
using UnityEngine;

public class LidgrenPeer : MonoBehaviour
{
    NetPeer peer = null;
    Dictionary<int, Action<NetIncomingMessage>> messageHandlers = new Dictionary<int, Action<NetIncomingMessage>>();

    protected event Action<NetConnection> Connected = null;
    protected event Action<NetConnection> Disconnected = null;

    protected void SetPeer(NetPeer peer)
    {
        this.peer = peer;
    }

    protected void RegisterMessageHandler(byte id, Action<NetIncomingMessage> message)
    {
        messageHandlers.Add(id, message);
    }

    protected void ReadMessages()
    {
        if (peer != null)
        {
            NetIncomingMessage msg;

            while ((msg = peer.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        byte messageId = msg.ReadByte();
                        Action<NetIncomingMessage> handler = null;

                        if (messageHandlers.TryGetValue(messageId, out handler))
                        {
                            handler(msg);
                        }
                        else
                        {
                            Debug.LogWarning("No handler for message id " + messageId);
                        }

                        break;

                    case NetIncomingMessageType.StatusChanged:
                        switch (msg.SenderConnection.Status)
                        {
                            case NetConnectionStatus.Connected:
                                if (Connected != null)
                                {
                                    Connected(msg.SenderConnection);
                                }
                                break;

                            case NetConnectionStatus.Disconnected:
                                if (Disconnected != null)
                                {
                                    Disconnected(msg.SenderConnection);
                                }
                                break;
                        }

                        Debug.Log("Status on " + msg.SenderConnection + " changed to " + msg.SenderConnection.Status);
                        break;

                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        Debug.Log("Lidgren: " + msg.ReadString());
                        break;

                }

                peer.Recycle(msg);
            }
        }
    }

    void FixedUpdate()
    {
        ReadMessages();
    }

    void Update()
    {
        ReadMessages();
    }

    void LateUpdate()
    {
        ReadMessages();
    }
}