// Author: Pietro Polsinelli - http://designAGame.eu
// Twitter https://twitter.com/ppolsinelli
// All free as in free beer :-)

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.EventSystems;

namespace OL
{
    public class TileManager : MonoBehaviour
    {
        [Header("Configuration")]
        public bool EnableDiagonals;
        public float CostToDistanceMultiplier=1;

        [Header("Others - computed")]
        public string DebugText;

        public Tile Selected;
        public Dictionary<Point, Tile> Tiles = new Dictionary<Point, Tile>();

        Dictionary<Vector3, Tile> positionTileCache = new Dictionary<Vector3, Tile>();
        
        public static event Action<Tile> TileClicked;
        public static void DispatchTileClicked(Tile type) { if (TileClicked != null) TileClicked(type); }
        
        public static TileManager I;

        void Awake()
        {
            if (I == null)
                I = this;
        }

        public void Setup()
        {
            Tile[] tiles = GetComponentsInChildren<Tile>();
            foreach (Tile tile in tiles)
            {
                //needed because you need the struct instance live at runtime
                tile.Setup();

                if (!Tiles.ContainsKey(tile.Point))
                    Tiles.Add(tile.Point, tile);
                else
                {
                    Debug.Log("duplicated " + tile.Point);
                }
            }
            DebugText = "tiles.Length "+tiles.Length;
        }

        public Tile FindNeighbourCostingLess(Tile t, float minimalCost, int maxDepth, int currentDepth = 0)
        {
            foreach (Tile neighbor in Neighbors(t))
            {
                if (neighbor.Cost <= minimalCost)
                    return neighbor;
            }
            currentDepth++;

            if (currentDepth <= maxDepth)
                foreach (Tile neighbor in Neighbors(t))
                {
                    if (neighbor==t)
                        continue;
                    Tile found = FindNeighbourCostingLess(neighbor, minimalCost, maxDepth, currentDepth);
                    if (found != null)
                        return found;
                }
            
            return null;
        }

        void OnMouseDown()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Tile tile = FindTileAtPosition(screenToWorldPoint);
                Debug.Log("found tile "+tile);
                DispatchTileClicked(tile);                
            }                
        }

        public Tile FindTileAtPosition(Vector3 position)
        {
            if (positionTileCache.ContainsKey(position))
                return positionTileCache[position];


            float minDist = float.MaxValue;
            Tile closest = null;
            foreach (Tile t in Tiles.Values)
            {
                if (t != null)
                {
                    Vector3 dis = (t.transform.position - position);
                    float planeDist = (Mathf.Abs(dis.x) + Mathf.Abs(dis.y));
                    if (closest == null || planeDist < minDist)
                    {
                        closest = t;
                        minDist = planeDist;
                        //Debug.Log("found close at " + t);
                    }
                }
            }
            positionTileCache[position] = closest;
            return closest;
        }

        public Point Point(Vector3 position)
        {
            float minDist = 0;
            Tile closest = null;
            foreach (Tile t in Tiles.Values)
            {
                float planeDist = Vector3.Distance(t.transform.position, position);
                if (closest == null || planeDist < minDist)
                {
                    closest = t;
                    minDist = planeDist;
                }
            }
            return closest.Point;
        }


        public List<Tile> Neighbors(Tile tile)
        {
            List<Tile> n = new List<Tile>();
            {
                Point left = new Point(tile.GridX - 1, tile.GridY);
                if (Tiles.ContainsKey(left))
                    n.Add(Tiles[left]);
                Point right = new Point(tile.GridX + 1, tile.GridY);
                if (Tiles.ContainsKey(right))
                    n.Add(Tiles[right]);
                Point above = new Point(tile.GridX, tile.GridY + 1);
                if (Tiles.ContainsKey(above))
                    n.Add(Tiles[above]);
                Point below = new Point(tile.GridX, tile.GridY - 1);
                if (Tiles.ContainsKey(below))
                    n.Add(Tiles[below]);
            }

            if (EnableDiagonals)
            {
                Point left = new Point(tile.GridX - 1, tile.GridY + 1);
                if (Tiles.ContainsKey(left))
                    n.Add(Tiles[left]);
                Point right = new Point(tile.GridX + 1, tile.GridY + 1);
                if (Tiles.ContainsKey(right))
                    n.Add(Tiles[right]);
                Point above = new Point(tile.GridX - 1, tile.GridY - 1);
                if (Tiles.ContainsKey(above))
                    n.Add(Tiles[above]);
                Point below = new Point(tile.GridX + 1, tile.GridY - 1);
                if (Tiles.ContainsKey(below))
                    n.Add(Tiles[below]);
            }
            return n;
        }

        // Adapted from http://www.redblobgames.com/pathfinding/a-star/implementation.html#csharp
        public Dictionary<Tile, Tile> AStar(Tile start, Tile goal)
        {
            Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
            Dictionary<Tile, double> costSoFar = new Dictionary<Tile, double>();

            var frontier = new PriorityQueue<Tile>();
            frontier.Enqueue(start, 0);

            cameFrom[start] = start;
            costSoFar[start] = 0;

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                if (current.Equals(goal))
                {
                    break;
                }

                foreach (var next in Neighbors(current))
                {
                    if (next.Cost < 1)
                    {
                        double newCost = costSoFar[current] + (next.Cost*CostToDistanceMultiplier);

                        if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                        {
                            costSoFar[next] = newCost;
                            double priority = newCost + next.DistanceFrom(goal);
                            frontier.Enqueue(next, priority);
                            cameFrom[next] = current;
                        }
                    }

                }
            }
            return cameFrom;
        }

        public static List<Tile> AStarFindPath(Tile start, Tile goal, Dictionary<Tile, Tile> cameFrom)
        {
            Tile current = goal;
            List<Tile> path = new List<Tile>();
            path.Add(current);
            while (current != start)
            {
                if (cameFrom.ContainsKey(current))
                {
                    current = cameFrom[current];
                    path.Add(current);
                }
                else
                {
                    Debug.Log("step not found in path " + current);
                    break;
                }
            }
            path.Reverse();
            return path;
        }

        public static List<Tile> AStarPath(Tile start, Tile goal)
        {
            Dictionary<Tile, Tile> resultingAStar = TileManager.I.AStar(start, goal);
            return AStarFindPath(start, goal, resultingAStar);
        }
        
        public class PriorityQueue<T>
        {
            private List<KeyValuePair<T, double>> elements = new List<KeyValuePair<T, double>>();

            public int Count
            {
                get { return elements.Count; }
            }

            public void Enqueue(T item, double priority)
            {
                KeyValuePair<T, double> keyValuePair = new KeyValuePair<T, double>(item, priority);
                elements.Add(keyValuePair);
            }

            public T Dequeue()
            {
                int bestIndex = 0;

                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].Value < elements[bestIndex].Value)
                    {
                        bestIndex = i;
                    }
                }

                T bestItem = elements[bestIndex].Key;
                elements.RemoveAt(bestIndex);
                return bestItem;
            }
        }

        public Vector3 Position(Point currentDestination)
        {
            return Tiles[currentDestination].transform.position;
        }
    }
}
