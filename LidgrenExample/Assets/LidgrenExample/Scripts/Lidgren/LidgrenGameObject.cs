using UnityEngine;
using System.Collections;
using Lidgren.Network;

public class LidgrenGameObject : MonoBehaviour
{
    public int Id;
    public bool IsMine;
    public NetConnection Connection { get; set; }

    void FixedUpdate()
    {
        if (IsMine)
        {
            NetOutgoingMessage msg = Connection.Peer.CreateMessage();
            msg.Write(LidgrenMessageHeaders.Position);
            msg.Write(Id);
            msg.Write(transform.position.x);
            msg.Write(transform.position.y);
            msg.Write(transform.position.z);

            Transform viking = transform.GetChild(0);
            msg.Write(viking.rotation.x);
            msg.Write(viking.rotation.y);
            msg.Write(viking.rotation.z);
            msg.Write(viking.rotation.w);

            Connection.SendMessage(msg, NetDeliveryMethod.Unreliable, 0);
        }
    }

    public static LidgrenGameObject Spawn(int myId, int id, NetConnection con)
    {
        GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("Player"), new Vector3(45, 10, 45), Quaternion.identity);
        LidgrenGameObject lgo = go.GetComponent<LidgrenGameObject>();

        lgo.Id = id;
        lgo.IsMine = myId == id;
        lgo.Connection = con;

        return lgo;
    }
}