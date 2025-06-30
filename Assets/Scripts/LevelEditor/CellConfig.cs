using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/CellConfig")]
public class CellConfig : ScriptableObject
{
    public List<CellData> configs;

    public CellData Get(string id)
    {
        return configs.Find(c => c.cellId == id);
    }

    public List<string> GetAllIds()
    {
        return configs.ConvertAll(c => c.cellId);
    }
}