using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private List<Sprite> icons;
    [SerializeField] private List<Cell> cells = new List<Cell>();
    public Cell cellPrefab;
    public Transform gridParent;

    public int gridSize = 7;
    [SerializeField] private float spacing = 110f;

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        cells = new List<Cell>();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                var cell = Instantiate(cellPrefab, gridParent);
                cell.x = x;
                cell.y = y;

                cells.Add(cell);

                int iconIndex = (x + y) % 2;
                var sprite = icons[iconIndex];
                cell.SetImage(sprite);
                float offsetX = -(gridSize - 1) * spacing / 2f;
                float offsetY = -(gridSize - 1) * spacing / 2f;

                cell.transform.localPosition = new Vector3(
                    offsetX + x * spacing,
                    offsetY + y * spacing,
                    0
                );
            }
        }
    }
}