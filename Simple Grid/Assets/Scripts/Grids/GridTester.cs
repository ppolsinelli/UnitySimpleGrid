// Author: Pietro Polsinelli - http://designAGame.eu
// Twitter https://twitter.com/ppolsinelli
// All free as in free beer :-)

using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace OL
{
    public class GridTester : MonoBehaviour
    {
        public Tile StartTile;
        private bool showingPaths;

        void Start()
        {
            TileManager.I.Setup();
            TileManager.TileClicked += TileClicked;

        }

        public void TileClicked(Tile t)
        {
            if (showingPaths)
                return;

            if (StartTile == null)
            {
                StartTile = t;
                StartTile.Sprite.color = Color.yellow;
            }
            else
            {
                t.Sprite.color = Color.red;
                showingPaths = true;
                DOVirtual.DelayedCall(1, () =>
                {
                    Dictionary<Tile, Tile> resultingAStar = TileManager.I.AStar(StartTile, t);
                    foreach (Tile tile in resultingAStar.Values)
                    {
                        if (tile != t && tile != StartTile)
                            tile.Sprite.color = Color.blue;
                    }

                    List<Tile> path = TileManager.AStarFindPath(StartTile, t, resultingAStar);
                    foreach (Tile tile in path)
                    {
                        if (tile != t && tile != StartTile)
                            tile.Sprite.color = Color.green;
                    }

                }).OnComplete(() =>
                {
                    DOVirtual.DelayedCall(2, () =>
                    {
                        foreach (Tile tile in TileManager.I.Tiles.Values)
                        {
                            tile.Sprite.color=tile.DefaultColor;
                        }
                        StartTile = null;
                        showingPaths = false;
                    });

                });
            }


        }



        void OnDisable()
        {
            TileManager.TileClicked -= TileClicked;
        }

    }
}
