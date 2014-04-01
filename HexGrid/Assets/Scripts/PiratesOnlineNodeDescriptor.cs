using UnityEngine;
using System.Collections;

public class PiratesOnlineNode
{
    public struct SubAtlas
    {
        public byte Atlas;
        public byte Tiles;
        public int[] Triangles;
        public byte AssignedTiles;
    }

    public struct Tile
    {
        public byte SubAtlas;
        public byte Icon;
    }

    public SubAtlas[] Atlases = null;
    public readonly Tile[] Tiles = new Tile[PiratesOnlineConstants.GridTiles];

    public PiratesOnlineNode(byte atlases)
    {
        Atlases = new SubAtlas[atlases];
    }
}