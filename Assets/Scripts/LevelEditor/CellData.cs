using LevelEditor;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class CellData
{
    public string cellId;
    public IntVec2 position;
    [JsonIgnore] public Sprite sprite;
}