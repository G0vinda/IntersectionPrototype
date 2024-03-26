using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class CityLayout : MonoBehaviour
{
    [Serializable]
    public class LayoutBlockData
    {    
        public int id { get; private set; }
        public string name { get; set; }
        public int[,] State;
        public List<NpcData> NpcState;
        public int difficulty;

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

        [JsonConstructor]
        public LayoutBlockData(string name, int difficulty)
        {
            State = new int[MaxGridX, MaxGridY];
            NpcState = new List<NpcData>();
            id = GetNewId();
            this.name = name;
            this.difficulty = difficulty;
        }

        public LayoutBlockData(string name, int[,] state, List<NpcData> npcState, int difficulty)
        {
            State = state;
            NpcState = npcState;
            id = GetNewId();
            this.name = name;
            this.difficulty = difficulty;
        }

        public static LayoutBlockData CopyData(string copyName, LayoutBlockData dataToCopy)
        {
            var copyState = dataToCopy.State.Clone() as int[,];
            var copyNpcState = new List<NpcData>();
            dataToCopy.NpcState.ForEach(npc => copyNpcState.Add(npc.Clone()));
            return new LayoutBlockData(copyName, copyState, copyNpcState, dataToCopy.difficulty);
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

        public NpcData Clone()
        {
            var clone = new NpcData(Coordinates, Direction)
            {
                Waypoints = Waypoints.ToList().ToArray()
            };

            return clone;
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

