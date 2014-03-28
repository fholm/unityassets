using UnityEngine;
using Lidgren.Network;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    Animation target = null;

    byte state = 0;
    Quaternion rotation = Quaternion.identity;
    LidgrenGameObject lgo = null;

    void Start()
    {
        // Setup animations
        target.wrapMode = WrapMode.Loop;

        target["Jump"].wrapMode = WrapMode.Once;
        target["Jump"].layer = 1;

        target["Land"].wrapMode = WrapMode.Once;
        target["Land"].layer = 1;

        target["Run"].speed = 1.75f;
        target["Walk"].speed = -1.25f;

        // always start with idle
        target.Play("Idle");

        lgo = GetComponent<LidgrenGameObject>();
    }

    void Update()
    {
        // Rotate
        if (lgo.IsMine)
        {
            switch (state)
            {
                case PlayerMovementEvent.Forward:
                    rotate(0);
                    break;

                case PlayerMovementEvent.Backward:
                    rotate(0);
                    break;

                case PlayerMovementEvent.Left:
                    rotate(-90);
                    break;

                case PlayerMovementEvent.Right:
                    rotate(90);
                    break;

                case PlayerMovementEvent.ForwardLeft:
                    rotate(-45);
                    break;

                case PlayerMovementEvent.ForwardRight:
                    rotate(45);
                    break;

                case PlayerMovementEvent.BackwardLeft:
                    rotate(45);
                    break;

                case PlayerMovementEvent.BackwardRight:
                    rotate(-45);
                    break;
            }
        }

        switch (state)
        {
            case PlayerMovementEvent.Idle:
                target.CrossFade("Idle");
                break;

            case PlayerMovementEvent.Fall:
                target.CrossFade("Fall");
                break;

            case PlayerMovementEvent.Forward:
            case PlayerMovementEvent.Left:
            case PlayerMovementEvent.Right:
            case PlayerMovementEvent.ForwardLeft:
            case PlayerMovementEvent.ForwardRight:
                target.CrossFade("Run");
                break;

            case PlayerMovementEvent.Backward:
            case PlayerMovementEvent.BackwardLeft:
            case PlayerMovementEvent.BackwardRight:
                target.CrossFade("Walk");
                break;
        }

        if (lgo.IsMine)
        {
            if (state == PlayerMovementEvent.Idle)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    target.transform.rotation = Quaternion.LookRotation(transform.forward);
                }
            }
            else
            {
                target.transform.rotation = Quaternion.Slerp(target.transform.rotation, rotation, Time.deltaTime * 10f);
            }
        }
    }

    void rotate(float yaw)
    {
        rotation = Quaternion.LookRotation(Quaternion.Euler(0, yaw, 0) * transform.forward, Vector3.up);
    }

    public void OnPlayerMovement(byte newState)
    {
        if (lgo.IsMine)
        {
            NetOutgoingMessage msg = lgo.Connection.Peer.CreateMessage();

            msg.Write(LidgrenMessageHeaders.Movement);
            msg.Write(lgo.Id);
            msg.Write(newState);

            lgo.Connection.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 1);
        }

        state = newState;

        switch (state)
        {
            case PlayerMovementEvent.Land:
                target.CrossFade("Land");
                break;

            case PlayerMovementEvent.Jump:
                target.CrossFade("Jump");
                break;
        }
    }
}


public static class PlayerMovementEvent
{
    public const byte Forward = 1;
    public const byte Backward = 2;
    public const byte Jump = 3;
    public const byte Fall = 4;
    public const byte Land = 5;
    public const byte Idle = 6;
    public const byte Left = 7;
    public const byte Right = 8;
    public const byte ForwardLeft = 9;
    public const byte ForwardRight = 10;
    public const byte BackwardLeft = 11;
    public const byte BackwardRight = 12;
}