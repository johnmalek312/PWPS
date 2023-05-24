﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PixelWorldsServer2.World
{
    public enum LayerType
    {
        Block,
        Background,
        Water,
        Wiring,
        Sign,
        Unknown
    }

    public struct WorldLayer
    {
        // straight taken from PW
        public short id; // item id
        public byte blockType, damage;
        public object anotherSprite; // ?
        public bool activeAnimation;
        public int direction;
        public int classId;
        public long lastHit;
    }

    public class WorldTile
    {
        WorldLayer[] layers;

        public ref WorldLayer fg => ref layers[(int)LayerType.Block];
        public ref WorldLayer bg => ref layers[(int)LayerType.Background];
        public ref WorldLayer water => ref layers[(int)LayerType.Water];
        public ref WorldLayer wire => ref layers[(int)LayerType.Wiring];

        public WorldTile()
        {
            layers = new WorldLayer[(int)LayerType.Unknown];
        }
    }
}
