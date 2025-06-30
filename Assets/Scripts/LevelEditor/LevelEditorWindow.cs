using System.Collections.Generic;
using System.IO;
using System.Text;
using LevelEditor;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR

public class LevelEditorWindow : EditorWindow
{
    private LevelData levelData;
    private int cellSize = 30;
    private CellConfig _config;
    private string currentId = "";

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }


    // UI
    private void OnGUI()
    {
        DrawTopMenuButtons();

        InitializeCellConfigDatabase();
        if (_config == null || _config.configs.Count == 0)
        {
            EditorGUILayout.HelpBox("CellConfigDatabase not set!", MessageType.Warning);
            return;
        }

        if (levelData == null)
            return;
        DrawGridSettings();
        GUILayout.Space(10);
        DrawGrid();
        ClearLevelButton();
        ExportToJsonButton();
    }

    private void DrawTopMenuButtons()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("New Level"))
        {
            levelData = new LevelData
            {
                gridSize = new IntVec2(8, 8),
                score = 0,
                levelId = "Level_" + System.DateTime.Now.ToString("yy_MM_dd_hh_mm_ss")
            };
            Repaint();
        }

        if (GUILayout.Button("Load Level from JSON"))
        {
            string path = EditorUtility.OpenFilePanel("Open Level JSON", "", "json");
            if (!string.IsNullOrEmpty(path))
            {
                string json = File.ReadAllText(path, Encoding.UTF8);
                var loadedCells = JsonConvert.DeserializeObject<List<CellData>>(json);
                levelData = new LevelData
                {
                    cells = loadedCells ?? new List<CellData>(),
                    gridSize = new IntVec2(8, 8),
                    score = 0
                };
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void InitializeCellConfigDatabase()
    {
        // Auto-find CellConfigDatabase asset if not set
        if (_config == null)
        {
            string[] guids = AssetDatabase.FindAssets($"t:CellConfig1");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _config = AssetDatabase.LoadAssetAtPath<CellConfig>(path);
            }
        }

        _config =
            (CellConfig)EditorGUILayout.ObjectField("Config DB", _config, typeof(CellConfig),
                false);
    }

    private void DrawGridSettings()
    {
        DrawBrushSelector();

        GUILayout.Label("Current Brush: " + currentId);
        levelData.gridSize = new IntVec2(
            EditorGUILayout.IntField("Grid Width", levelData.gridSize.x),
            EditorGUILayout.IntField("Grid Height", levelData.gridSize.y)
        );
        levelData.score = EditorGUILayout.IntField("Score", levelData.score);
        levelData.levelId = EditorGUILayout.TextField("LevelId", levelData.levelId);
    }

    private void DrawBrushSelector()
    {
        GUILayout.Label("Select Brush (Cell):", EditorStyles.boldLabel);

        int columns = 5;
        int size = 48;
        int rows = Mathf.CeilToInt(_config.configs.Count / (float)columns);

        for (int row = 0; row < rows; row++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int col = 0; col < columns; col++)
            {
                int i = row * columns + col;
                if (i >= _config.configs.Count) break;

                var cell = _config.configs[i];
                Texture2D preview = cell.sprite != null
                    ? AssetPreview.GetAssetPreview(cell.sprite)
                    : Texture2D.whiteTexture;
                GUIContent content = new GUIContent(preview, cell.cellId);

                GUIStyle style = GUI.skin.button;
                if (GUILayout.Button(content, style, GUILayout.Width(size), GUILayout.Height(size)))
                {
                    currentId = cell.cellId;
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void ExportToJsonButton()
    {
        if (GUILayout.Button("Export to JSON"))
        {
            string path = EditorUtility.SaveFilePanel("Save Level JSON", "", "LevelData.json", "json");
            if (!string.IsNullOrEmpty(path))
            {
                string json = JsonConvert.SerializeObject(levelData);
                File.WriteAllText(path, json, Encoding.UTF8);
                Debug.Log("Level exported to: " + path);
                AssetDatabase.Refresh();
            }
        }
    }

    private void ClearLevelButton()
    {
        GUILayout.Space(10);
        if (GUILayout.Button("Clear Level"))
        {
            if (EditorUtility.DisplayDialog("Clear?", "Are you sure you want to clear all data?", "Yes", "No"))
            {
                levelData.cells.Clear();
                levelData.score = 0;
                levelData.gridSize = new IntVec2(8, 8);
            }
        }
    }

    // Drawing
    private void DrawGrid()
    {
        Rect rect = GUILayoutUtility.GetRect(levelData.gridSize.x * cellSize, levelData.gridSize.y * cellSize);

        for (int y = 0; y < levelData.gridSize.y; y++)
        {
            for (int x = 0; x < levelData.gridSize.x; x++)
            {
                IntVec2 pos = new IntVec2(x, y);
                Rect cellRect = new(rect.x + x * cellSize, rect.y + y * cellSize, cellSize - 2, cellSize - 2);
                DrawGridCell(pos, cellRect);
                HandleGridClick(pos, cellRect);
            }
        }
    }

    private void DrawGridCell(IntVec2 pos, Rect cellRect)
    {
        CellData cell = levelData.GetCell(pos);
        Color color = Color.black;

        if (_config != null && cell != null)
        {
            var config = _config.Get(cell.cellId);
            if (config?.sprite != null)
            {
                GUI.DrawTexture(cellRect, config.sprite.texture, ScaleMode.ScaleToFit);
                return;
            }
        }

        EditorGUI.DrawRect(cellRect, color);
    }

    private void HandleGridClick(IntVec2 pos, Rect cellRect)
    {
        if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
        {
            ToggleCell(pos);
            Event.current.Use();
        }
    }

    private void ToggleCell(Vector2Int pos)
    {
        var config = _config.Get(currentId);
        if (config == null) return;

        CellData cell = levelData.GetCell(pos);
        if (cell == null)
        {
            levelData.cells.Add(new CellData
            {
                position = pos,
                cellId = config.cellId,
                sprite = config.sprite
            });
        }
        else
        {
            if (config.cellId == "empty")
            {
                levelData.cells.Remove(cell);
            }
            else
            {
                cell.cellId = config.cellId;
                cell.sprite = config.sprite;
            }
        }
    }
}
#endif