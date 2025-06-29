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
    private CellConfigDatabase configDatabase;
    private string currentId = "";

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    private void OnGUI()
    {
        levelData = (LevelData)EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelData), false);

        configDatabase =
            (CellConfigDatabase)EditorGUILayout.ObjectField("Config DB", configDatabase, typeof(CellConfigDatabase),
                false);

        if (configDatabase == null || configDatabase.configs.Count == 0)
        {
            EditorGUILayout.HelpBox("CellConfigDatabase не задано!", MessageType.Warning);
            return;
        }

        var allIds = configDatabase.GetAllIds();
        int selectedIndex = Mathf.Max(0, allIds.IndexOf(currentId));
        selectedIndex = EditorGUILayout.Popup("Brush ID", selectedIndex, allIds.ToArray());
        currentId = allIds[selectedIndex];
        levelData.gridSize = new IntVec2(
            EditorGUILayout.IntField("Grid Width", levelData.gridSize.x),
            EditorGUILayout.IntField("Grid Height", levelData.gridSize.y)
        );
        if (levelData == null) return;

        GUILayout.Space(10);
        DrawGrid();

        GUILayout.Space(10);
        if (GUILayout.Button("Clear Level"))
        {
            if (EditorUtility.DisplayDialog("Clear?", "Are you sure you want to clear all data?", "Yes", "No"))
            {
                levelData.cells.Clear();
                EditorUtility.SetDirty(levelData);
            }
        }

        if (GUILayout.Button("Export to JSON"))
        {
            string path = EditorUtility.SaveFilePanel("Save Level JSON", "", "LevelData.json", "json");
            if (!string.IsNullOrEmpty(path))
            {
                string json = JsonConvert.SerializeObject(levelData.cells);
                File.WriteAllText(path, json, Encoding.UTF8);
                Debug.Log("Level exported to: " + path);
            }
        }
    }

    private void DrawGrid()
    {
        Rect rect = GUILayoutUtility.GetRect(levelData.gridSize.x * cellSize, levelData.gridSize.y * cellSize);

        for (int y = 0; y < levelData.gridSize.y; y++)
        {
            for (int x = 0; x < levelData.gridSize.x; x++)
            {
                IntVec2 pos = new IntVec2(x, y);
                Rect cellRect = new(rect.x + x * cellSize, rect.y + y * cellSize, cellSize - 2, cellSize - 2);

                CellData cell = levelData.GetCell(pos);

                Color color = Color.black;

                if (configDatabase != null && cell != null)
                {
                    var config = configDatabase.Get(cell.cellId);
                    if (config?.sprite != null)
                    {
                        GUI.DrawTexture(cellRect, config.sprite.texture, ScaleMode.ScaleToFit);
                    }
                    else
                    {
                        EditorGUI.DrawRect(cellRect, color);
                    }
                }
                else
                {
                    EditorGUI.DrawRect(cellRect, color);
                }


                if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
                {
                    ToggleCell(pos);
                    Event.current.Use();
                }
            }
        }
    }

    private void ToggleCell(Vector2Int pos)
    {
        var config = configDatabase.Get(currentId);
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

        EditorUtility.SetDirty(levelData);
    }
}
#endif