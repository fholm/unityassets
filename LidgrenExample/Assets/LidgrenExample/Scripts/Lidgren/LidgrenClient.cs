using UnityEngine;
using System.Collections;
using Lidgren.Network;
using System.Collections.Generic;

public class LidgrenClient : LidgrenPeer
{
    int clientId;
    NetClient client;
    Dictionary<int, LidgrenGameObject> lgos = new Dictionary<int, LidgrenGameObject>();

    [SerializeField]
    int port = 10000;

    [SerializeField]
    string host = "127.0.0.1";

    void Start()
    {
        GameObject.DontDestroyOnLoad(this);
        GameObject.DontDestroyOnLoad(gameObject);

        client = new NetClient(new NetPeerConfiguration("LidgrenDemo"));
        client.Start();
        client.Connect(host, port);

        SetPeer(client);

        Connected += onConnected;
        Disconnected += onDisconnected;

        RegisterMessageHandler(LidgrenMessageHeaders.Hello, onHello);
        RegisterMessageHandler(LidgrenMessageHeaders.Spawn, onSpawn);
        RegisterMessageHandler(LidgrenMessageHeaders.Despawn, onDespawn);
        RegisterMessageHandler(LidgrenMessageHeaders.Movement, onMovement);
        RegisterMessageHandler(LidgrenMessageHeaders.Position, onPosition);
    }

    void onConnected(NetConnection c)
    {
        Debug.Log("Connected to server");
    }

    void onDisconnected(NetConnection c)
    {
        Debug.Log("Disconnected from server");
    }

    void onMovement(NetIncomingMessage msg)
    {
        Debug.Log("onMovement");

        int id = msg.ReadInt32();
        LidgrenGameObject lgo = null;

        if (lgos.TryGetValue(id, out lgo))
        {
            lgo.GetComponent<PlayerAnimator>().OnPlayerMovement(msg.ReadByte());
        }
    }

    void onPosition(NetIncomingMessage msg)
    {
        Debug.Log("onPosition");

        int id = msg.ReadInt32();
        LidgrenGameObject lgo = null;

        if (lgos.TryGetValue(id, out lgo))
        {
            // Update position
            Vector3 pos = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
            lgo.transform.position = pos;

            // Update rotation
            Quaternion rot = new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
            lgo.transform.GetChild(0).rotation = rot;
        }
    }

    void onSpawn(NetIncomingMessage msg)
    {
        int id = msg.ReadInt32();
        lgos.Add(id, LidgrenGameObject.Spawn(clientId, id, msg.SenderConnection));
    }

    void onDespawn(NetIncomingMessage msg)
    {
        try
        {
            int id = msg.ReadInt32();
            GameObject.Destroy(lgos[id]);
            lgos.Remove(id);
        }
        catch
        {

        }
    }

    void onHello(NetIncomingMessage msg)
    {
        clientId = msg.ReadInt32();

        NetOutgoingMessage spawn = msg.SenderConnection.Peer.CreateMessage();
        spawn.Write(LidgrenMessageHeaders.RequestSpawn);

        msg.SenderConnection.SendMessage(spawn, NetDeliveryMethod.ReliableOrdered, 1);
    }
}