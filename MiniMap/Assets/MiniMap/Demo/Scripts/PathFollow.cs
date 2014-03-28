using System.Linq;
using UnityEngine;

public class PathFollow : MonoBehaviour
{
    PathMover mover;

    public float MoveSpeed = 1f;
    public Vector3[] Path = new Vector3[0];

    void Start()
    {
        mover = new PathMover(0);
        mover.Path = Path;
        mover.PathLength = Path.Length;
        mover.StartMoving(Time.time, MoveSpeed);
    }

    void Update()
    {
        transform.position = mover.GetPosition(Time.time, transform.position);

        if (mover.IsDone)
        {
            mover.Path = mover.Path.Reverse().ToArray();
            mover.StartMoving(Time.time, MoveSpeed);
        }
    }
}

public class PathMover
{
    public Vector3[] Path;
    public int PathLength;
    public Vector3 MoveDirection;
    public bool IsDone;
    public float LastMoveTime;

    float moveSpeed;
    float moveStartTime;
    float smoothTime;
    float stopAtTime;
    float timeMoved;
    int iterations;

    Vector3 smoothTo;
    Vector3 smoothFrom;

    public Vector3 LastWaypoint
    {
        get
        {
            if (PathLength > 0)
                return Path[PathLength - 1];

            return new Vector3(float.MaxValue, 0, 0);
        }
    }

    public Vector3 FirstWaypoint
    {
        get
        {
            if (PathLength > 0)
                return Path[0];

            return new Vector3(float.MaxValue, 0, 0);
        }
    }

    public PathMover(int initialSize)
    {
        Path = new Vector3[initialSize];
        IsDone = true;
    }

    public void StartMoving(float moveStartTime, float moveSpeed)
    {
        this.moveStartTime = moveStartTime;
        this.moveSpeed = moveSpeed;
        this.smoothTime = 0f;
        this.stopAtTime = 0f;
        this.IsDone = false;
        this.iterations = 0;
    }

    public void StartMoving(float moveStartTime, float moveSpeed, Vector3 smoothFrom, float smoothTime)
    {
        StartMoving(moveStartTime, moveSpeed);

        this.smoothTo = Path[0];
        this.smoothFrom = smoothFrom;
        this.smoothTime = smoothTime;
    }

    public void Stop()
    {
        Stop(-1f);
    }

    public void Stop(float inSeconds)
    {
        if (stopAtTime != 0f)
            return;

        stopAtTime = timeMoved + inSeconds;
    }

    public Vector3 GetPosition(float currentTime, Vector3 position)
    {
        var y = position.y;

        iterations++;
        timeMoved = currentTime - moveStartTime;

        if (IsDone || (stopAtTime != 0f && stopAtTime <= timeMoved))
        {
            IsDone = true;
            return position;
        }

        if (Path == null || PathLength < 1)
        {
            IsDone = true;
            MoveDirection = Vector3.zero;
            return position;
        }

        LastMoveTime = currentTime;

        if (smoothTime != 0.0f)
        {
            Path[0] = Vector3.Lerp(smoothFrom, smoothTo, timeMoved / smoothTime);
        }

        float distance;
        Vector3 currentPosition;

        currentPosition = position;
        distance = timeMoved * moveSpeed;
        position = Path[0];

        var clamped = Vector3.zero;
        var direction = Vector3.zero;

        for (var i = 0; i + 1 < PathLength; ++i)
        {
            direction = Path[i + 1] - Path[i];
            if (direction.magnitude > distance)
            {
                clamped = Vector3.ClampMagnitude(direction, distance);
                position = position + clamped;

                break;
            }
            else
            {
                distance -= direction.magnitude;
                position += direction;
            }
        }

        IsDone = (Path[PathLength - 1] - position).magnitude < 0.001f;
        MoveDirection = (position - currentPosition);
        MoveDirection.Normalize();

        position.y = y;

        return position;
    }
}