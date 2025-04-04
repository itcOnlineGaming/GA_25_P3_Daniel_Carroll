using UnityEngine;
using UnityEditor;

public class WorldBuilderWindow : EditorWindow
{
    private Simple2DWorldBuilder activeBuilder;
    private Vector2 scrollPos;

    [MenuItem("Tools/World Builder Window")]
    public static void ShowWindow()
    {
        var window = GetWindow<WorldBuilderWindow>("World Builder");
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Active Builder", EditorStyles.boldLabel);
        activeBuilder = EditorGUILayout.ObjectField("Builder Object",
                        activeBuilder, typeof(Simple2DWorldBuilder), true)
                        as Simple2DWorldBuilder;

        if (activeBuilder == null)
        {
            EditorGUILayout.HelpBox("Please assign a Simple2DWorldBuilder instance.", MessageType.Info);
            return;
        }

        EditorGUILayout.Space();

        // BRUSH RADIUS SLIDER
        EditorGUILayout.LabelField("Brush Settings", EditorStyles.boldLabel);
        activeBuilder.brushRadius = EditorGUILayout.IntSlider("Brush Radius", activeBuilder.brushRadius, 0, 5);

        EditorGUILayout.Space();

        // TILE PALETTE
        EditorGUILayout.LabelField("Tile Palette", EditorStyles.boldLabel);
        if (activeBuilder.tilePrefabs == null || activeBuilder.tilePrefabs.Length == 0)
        {
            EditorGUILayout.HelpBox("No tile prefabs set in the builder!", MessageType.Warning);
        }
        else
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(100));
            for (int i = 0; i < activeBuilder.tilePrefabs.Length; i++)
            {
                var prefab = activeBuilder.tilePrefabs[i];
                if (prefab == null) continue;

                EditorGUILayout.BeginHorizontal("box");
                Texture2D previewTex = AssetPreview.GetAssetPreview(prefab);
                GUILayout.Label(previewTex, GUILayout.Width(50), GUILayout.Height(50));
                EditorGUILayout.LabelField(prefab.name);
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    activeBuilder.selectedTileIndex = i;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space();

        // QUICK ACTIONS
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
        if (GUILayout.Button("Clear All Tiles"))
        {
            if (EditorUtility.DisplayDialog("Clear All Tiles",
                "Are you sure you want to remove all placed tiles?", "Yes", "Cancel"))
            {
                ClearAllTiles();
            }
        }

        if (GUILayout.Button("Recalculate Tiles (Positions/Scales)"))
        {
            activeBuilder.RecalculateAllTiles();
        }

        if (GUILayout.Button("Repaint Scene View"))
        {
            SceneView.RepaintAll();
        }
    }

    private void ClearAllTiles()
    {
        if (activeBuilder == null) return;

        foreach (var kvp in activeBuilder.placedTiles)
        {
            if (kvp.Value != null)
            {
#if UNITY_EDITOR
                Undo.DestroyObjectImmediate(kvp.Value);
#else
                DestroyImmediate(kvp.Value);
#endif
            }
        }
        activeBuilder.placedTiles.Clear();
    }
}
