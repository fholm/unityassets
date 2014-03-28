using UnityEngine;

public abstract class FogOfWarLineOfSightChecker : MonoBehaviour
{
    public abstract bool HasLineOfSight(FogOfWar fow, int fromRow, int fromCol, int toRow, int toCol, float yPos, float maxHeight);
}
