using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Cell : MonoBehaviour
{
    [SerializeField] private Image icon;

    private int _x;
    private int _y;
    public string elementType;
    private GridManager _gridManager;

    [Inject]
    public void Construct(GridManager gridManager)
    {
        _gridManager = gridManager;
    }

    public void Setup(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public void SetImage(Sprite sprite)
    {
        icon.sprite = sprite;
    }
}