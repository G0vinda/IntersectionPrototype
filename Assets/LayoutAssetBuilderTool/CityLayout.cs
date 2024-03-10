using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEngine;

public class CityLayout : MonoBehaviour
{
    [Serializable]
    public class LayoutBlockData
    {    
        public int[,] State;
        public List<NpcData> NpcState;
        public int id { get; private set; }
        public string name { get; set; }

        public const int MaxGridX = 9;
        public const int MaxGridY = 6;

        private static List<int> idsInUse = new List<int>();

        public static int GetNewId()
        {
            int randomId;
            do
            {
                randomId = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            }while(idsInUse.Contains(randomId));

            idsInUse.Add(randomId);
            return randomId;
        }

        public static void RemoveId(int idToRemove)
        {
            idsInUse.Remove(idToRemove); 
        }

        public LayoutBlockData(string name)
        {
            State = new int[MaxGridX, MaxGridY];
            NpcState = new List<NpcData>();
            id = GetNewId();
            this.name = name;
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

