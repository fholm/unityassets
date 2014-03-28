using UnityEngine;
using System.Collections;
using Lidgren.Network;
using System.Linq;
using System.Collections.Generic;

public class LidgrenServer : LidgrenPeer
{
    int clientCounter = 0;
    NetServer server = null;

    [SerializeField]
    int port = 10000;

    void Start()
    {
        GameObject.DontDestroyOnLoad(this);
        GameObject.DontDestroyOnLoad(gameObject);

        NetPeerConfiguration config = new NetPeerConfiguration("LidgrenDemo");
        config.Port = port;

        server = new NetServer(config);
        server.Start();

        SetPeer(server);

        Connected += onConnected;
        Disconnected += onDisconnected;

        RegisterMessageHandler(LidgrenMessageHeaders.RequestSpawn, onRequestSpawn);
        RegisterMessageHandler(LidgrenMessageHeaders.Movement, onMovement);
        RegisterMessageHandler(LidgrenMessageHeaders.Position, onPosition);
    }

    void spawnOn(LidgrenGameObject go, NetConnection c)
    {
        NetOutgoingMessage msg = c.Peer.CreateMessage();
        msg.Write(LidgrenMessageHeaders.Spawn);
        msg.Write(go.Id);

        c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 1);
    }

    void onMovement(NetIncomingMessage msg)
    {
        LidgrenPlayer player = (LidgrenPlayer)msg.SenderConnection.Tag;

        // Forward message to all other players
        NetOutgoingMessage forward = msg.SenderConnection.Peer.CreateMessage();
        forward.Write(msg);

        server.SendToAll(forward, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 1);
        
        // Throw away Id on server, we dont need it
        msg.ReadInt32();

        // Read movement animation on server also
        player.GameObject.GetComponent<PlayerAnimator>().OnPlayerMovement(msg.ReadByte());
    }

    void onPosition(NetIncomingMessage msg)
    {
        LidgrenPlayer player = (LidgrenPlayer)msg.SenderConnection.Tag;

        // Forward message to all other players
        NetOutgoingMessage forward = msg.SenderConnection.Peer.CreateMessage();
        forward.Write(msg);

        server.SendToAll(forward, msg.SenderConnection, NetDeliveryMethod.Unreliable, 0);

        // Throw away Id on server, we dont need it
        msg.ReadInt32();

        // Update position
        Vector3 pos = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
        player.GameObject.transform.position = pos;

        // Update rotation
        Quaternion rot = new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
        player.GameObject.transform.GetChild(0).rotation = rot;
    }

    void onRequestSpawn(NetIncomingMessage msg)
    {
        LidgrenPlayer player = (LidgrenPlayer)msg.SenderConnection.Tag;

        if (player.GameObject == null)
        {
            player.GameObject = LidgrenGameObject.Spawn(-1, player.Id, msg.SenderConnection);

            foreach (NetConnection c in server.Connections)
            {
                spawnOn(player.GameObject, c);
            }
        }
    }

    void onConnected(NetConnection c)
    {
        NetOutgoingMessage msg = c.Peer.CreateMessage();
        msg.Write(LidgrenMessageHeaders.Hello);
        msg.Write(++clientCounter);

        c.Tag = new LidgrenPlayer(clientCounter);
        c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 1);

        foreach (LidgrenGameObject go in FindObjectsOfType(typeof(LidgrenGameObject)).Cast<LidgrenGameObject>())
        {
            spawnOn(go, c);
        }

        Debug.Log("Client connected");
    }

    void onDisconnected(NetConnection c)
    {
        LidgrenPlayer player = (LidgrenPlayer)c.Tag;

        NetOutgoingMessage msg = server.CreateMessage();
        msg.Write(LidgrenMessageHeaders.Despawn);
        msg.Write(player.Id);

        server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);

        if (player.GameObject != null)
        {
            GameObject.Destroy(player.GameObject);
        }

        Debug.Log("Client disconnected");
    }
}