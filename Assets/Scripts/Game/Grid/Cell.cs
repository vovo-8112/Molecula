using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int x, y;
    public Image icon;
    public string elementType;

    public void SetImage(Sprite sprite)
    {
        icon.sprite = sprite;
    }
}