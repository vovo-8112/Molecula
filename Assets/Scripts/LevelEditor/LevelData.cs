using System;
using System.Collections.Generic;
using LevelEditor;

[Serializable]
public class LevelData
{
    public IntVec2 gridSize;
    public int score;
    public string levelId;
    public List<CellData> cells = new();

    public CellData GetCell(IntVec2 position)
    {
        return cells.Find(c => c.position == position);
    }
}