using UnityEngine;
using System.Collections;

public static class PiratesOnlineConstants
{
    public const float MinFov = 20;
    public const float MaxFov = 60;

    public const int WorldSize = 4096;
    public const int GridCount = WorldSize / GridSize;
    public const int GridSize = 8;
    public const int GridTiles = GridSize * GridSize;
    public const float GridWidth = SideLength * 1.5f * GridSize;
    public const float GridHeight = Inradius * 2.0f * GridSize;
    public const int SpawnDistance = 2;

    public const float SideLength = 1;
    public const float Inradius = 0.5f * PiratesOnlineMath.Sqrt3 * SideLength;
    public const int AtlasPixelSize = 1024;
    public const int IconPixelSize = 64;
    public const float IconUvSize = 1f / (float)IconPixelSize;
    public const float OnePixelUvSize = 1f / (float)AtlasPixelSize;
    public const int IndicesPerHexagon = 12;
    public const int VerticesPerHexagon = 6;
}