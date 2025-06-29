using System;
using UnityEngine;

namespace LevelEditor
{
    [Serializable]
    public struct IntVec2 
    {
        public int x;
        public int y;

        public IntVec2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        
        public static bool operator ==(IntVec2 a, IntVec2 b) => a.x == b.x && a.y == b.y;
        public static bool operator !=(IntVec2 a, IntVec2 b) => !(a == b);

        public static implicit operator IntVec2(Vector2Int v)
        {
            return new IntVec2(v.x, v.y);
        }

        public static implicit operator Vector2Int(IntVec2 v)
        {
            return new Vector2Int(v.x, v.y);
        }

        public bool Equals(IntVec2 other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            return obj is IntVec2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
    }
}