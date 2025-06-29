using System.Collections.Generic;
using LevelEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public List<CellData> cells = new();
    public IntVec2 gridSize;

    public CellData GetCell(IntVec2 position)
    {
        return cells.Find(c => c.position == position);
    }
}