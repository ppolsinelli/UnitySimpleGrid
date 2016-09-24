// Author: Pietro Polsinelli - http://designAGame.eu
// Twitter https://twitter.com/ppolsinelli
// All free as in free beer :-)

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace OL
{
    public class GridEditorTool : EditorWindow
    {
        private int width;
        private int height;
        private Transform root;
        private Transform rowPrefab;
        private Tile tilePrefab;
        private Color oddColor;
        private Color evenColor;


        [MenuItem("Tools/OL/GridGenerator")]
        private static void Init()
        {
            GridEditorTool window = (GridEditorTool)EditorWindow.GetWindow(typeof(GridEditorTool));
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Creates a grid", EditorStyles.boldLabel);

            oddColor = EditorGUILayout.ColorField("oddColor", oddColor);
            evenColor = EditorGUILayout.ColorField("evenColor", evenColor);
            root = EditorGUILayout.ObjectField("root", root, typeof(Transform), true) as Transform;
            rowPrefab = EditorGUILayout.ObjectField("rowPrefab", rowPrefab, typeof(Transform), true) as Transform;
            tilePrefab = EditorGUILayout.ObjectField("tilePrefab", tilePrefab, typeof(Tile), true) as Tile;


            width = EditorGUILayout.IntField("X", width);
            height = EditorGUILayout.IntField("Y", height);

            if (GUILayout.Button("create grid"))
            {
                Debug.Log("Create grid " + width + " " + height);

                for (int y = 0; y < height; y++)
                {
                    Transform row = PrefabUtility.InstantiatePrefab(rowPrefab) as Transform;
                    row.SetParent(root, false);

                    row.gameObject.name = y.ToString();
                    row.transform.localPosition = row.transform.localPosition +
                                                         new Vector3(0, y - (height / 2), 0);

                    for (int x = 0; x < width; x++)
                    {
                        Tile newTile = PrefabUtility.InstantiatePrefab(tilePrefab) as Tile;
                        newTile.transform.SetParent(row, false);
                        newTile.gameObject.name = x.ToString();
                        newTile.transform.localPosition = newTile.transform.localPosition +
                            new Vector3(x - (width / 2), 0, 0);
                        newTile.SetupByEditor(oddColor, evenColor);
                    }
                }
            }
        }
    }
}
