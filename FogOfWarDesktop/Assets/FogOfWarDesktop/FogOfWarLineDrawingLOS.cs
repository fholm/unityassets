using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogOfWarLineDrawingLOS : FogOfWarLineOfSightChecker
{
    public override bool HasLineOfSight(FogOfWar fow, int fromRow, int fromCol, int toRow, int toCol, float yPos, float maxHeight)
    {
        return hasLOS(fow, fromRow, fromCol, toRow, toCol, yPos, maxHeight);
    }

    bool hasLOS(FogOfWar fow, int x0, int y0, int x1, int y1, float y, float h)
    {
        int nodeCount = fow.NodeCount;
        float[] fogHeight = fow.HeightData;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            if (fogHeight != null && !(fogHeight[(x0 * nodeCount) + y0] - y < h))
            {
                return false;
            }

            if (x0 == x1 && y0 == y1)
            {
                return true;
            }

            int e2 = 2 * err;

            if (e2 > -dy)
            {
                err = err - dy;
                x0 = x0 + sx;
            }

            if (e2 < dx)
            {
                err = err + dx;
                y0 = y0 + sy;
            }
        }
    }
}