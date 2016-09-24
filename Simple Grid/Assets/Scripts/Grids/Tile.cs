// Author: Pietro Polsinelli - http://designAGame.eu
// Twitter https://twitter.com/ppolsinelli
// All free as in free beer :-)

using System;
using DG.Tweening;
using OL;
using UnityEngine;

namespace OL
{
    public class Tile : MonoBehaviour
    {
        public int GridX;
        public int GridY;
        public Point Point;
        // 1 not walkable, 0 walkable
        public float Cost;
        public Color DefaultColor;

        public SpriteRenderer Sprite;
        public Vector2 Position;

        public void SetupByEditor(Color oddColor, Color evenColor)
        {
            Sprite = GetComponent<SpriteRenderer>();
            GridX = Int32.Parse(name);
            GridY = Int32.Parse(transform.parent.name);
            Point = new Point(GridX, GridY);
            ResetColor(oddColor, evenColor);
        }

        public void Setup()
        {
            name = "Tile " + GridX + " " + GridY;
            Point = new Point(GridX, GridY);
        }

        public void ResetColor(Color oddColor, Color evenColor)
        {
            if ((GridX + GridY)%2==0)
                Sprite.color = evenColor;
            else
                Sprite.color = oddColor;

            DefaultColor = Sprite.color;
        }



        public float DistanceFrom(Tile tile)
        {
            return tile.Point.Distance(Point);
        }

        public void ColorByCost()
        {
            if (Mathf.Approximately(Cost, 0))
            {
                Sprite.color = DefaultColor;
            }
            else if (Cost > 0 && Cost < .9f)
            {
                Sprite.color = Color.magenta;
            }
            else if (Cost > .9f)
            {
                Sprite.color = Color.black;
            }
        }

    }
}
