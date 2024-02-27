using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityLayout : MonoBehaviour
{
    [Serializable]
    public class LayoutBlockData
    {
        public int[,] State;
        public List<NpcData> NpcState;

        public LayoutBlockData(int maxX, int maxY)
        {
            State = new int[maxX, maxY];
            NpcState = new List<NpcData>();
        }
    }
    
    public class NpcData
    {
        public Vector2Int Coordinates;
        public Vector2Int[] Waypoints;
        public Direction Direction;

        public NpcData(Vector2Int coordinates, Direction direction)
        {
            Coordinates = coordinates;
            Waypoints = new Vector2Int[] { };
            Direction = direction;
        }
    }

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    public enum BuildingType
    {
        Normal,
        Water,
        Park
    }

    public enum BetweenPartType
    {
        Normal,
        Blocked,
        Tunnel,
        Water,
        Park
    }

    public enum IntersectionType
    {
        Normal,
        Water
    }
}

