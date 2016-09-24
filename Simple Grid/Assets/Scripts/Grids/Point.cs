// Author: Pietro Polsinelli - http://designAGame.eu
// Twitter https://twitter.com/ppolsinelli
// All free as in free beer :-)

using System;
using UnityEngine;

namespace OL
{
    public struct Point
    {
        public int X;
        public int Y;

        public static Point North = new Point(0, 1);
        public static Point South = new Point(0, -1);
        public static Point East = new Point(1, 0);
        public static Point West = new Point(-1, 0);

        public Point(int gridX, int gridY)
        {
            X = gridX;
            Y = gridY;
        }

        public override string ToString()
        {
            return X + " " + Y;
        }

        public float Distance(Point point)
        {
            float distance = Mathf.Sqrt(Mathf.Pow(point.X - X, 2) + Mathf.Pow(point.Y - Y, 2));
            return distance;
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

    }
}
