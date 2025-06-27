using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BallCell : Cell, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    private bool isSelected = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        // Починаємо вибір ланцюга
        isSelected = true;
        // GridManager.Instance.StartSelection(this);
        Select();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected)
        {
            // Продовжуємо вибір, якщо пальцем рухаємось по кульках
            // GridManager.Instance.ContinueSelection(this);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isSelected)
        {
            isSelected = false;
            // GridManager.Instance.EndSelection();
        }
    }

    public void Select()
    {
        icon.color = Color.yellow;
    }

    public void Deselect()
    {
        icon.color = Color.white;
    }
}