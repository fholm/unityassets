using UnityEngine;
using System.Collections;

public class BattleTextAnimation : ScriptableObject
{
    internal bool HasRandomPosition;

    public bool AnimateInWorld = false;
    public float Time = 2f;
    public float FadeDelay = 1f;
    public Color DefaultColor = new Color(1, 1, 1, 0.5f);
    public Vector3 FixedOffset;
    public Vector3 RandomOffset;
    public BattleTextSizeAnimation[] Size;
    public BattleTextPositionAnimation[] Position;

    public BattleTextPositionAnimation[] ClonePosition()
    {
        int length = Position.Length;

        for (int i = 0; i < length; ++i)
        {
            if (Position[i].Random)
            {
                HasRandomPosition = true;
                BattleTextPositionAnimation[] clone = new BattleTextPositionAnimation[Position.Length];

                for (int j = 0; j < length; ++j)
                {
                    clone[j] = new BattleTextPositionAnimation();
                    clone[j].Random = Position[j].Random;
                    clone[j].Amount = Position[j].Amount;
                    clone[j].Time = Position[j].Time;
                    clone[j].Start = Position[j].Start;
                    clone[j].Falloff = Position[j].Falloff;
                }

                return clone;
            }
        }

        return Position;
    }
}

[System.Serializable]
public class BattleTextPositionAnimation
{
    public Vector3 Amount;
    public bool Random = false;
    public float Time;
    public float Start;
    public float Falloff;
}

[System.Serializable]
public class BattleTextSizeAnimation
{
    public float From;
    public float To;
    public float Time;
}