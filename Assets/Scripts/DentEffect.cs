using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DentEffect : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public float SelectedBent = 1;
    public float PressedBent = 2;

    private List<(RectTransform, Vector2)> objects = new List<(RectTransform, Vector2)>();

    private bool entered = false;
    private bool selected = false;
    private bool pressed = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        entered = true;
        Invalidate();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        entered = false;
        Invalidate();
    }

    public void OnSelect(BaseEventData eventData)
    {
        selected = true;
        Invalidate();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selected = false;
        Invalidate();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        Invalidate();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        Invalidate();
    }

    private void OnEnable()
    {
        objects.Clear();
        RectTransform[] comps = GetComponentsInChildren<RectTransform>();
        foreach (RectTransform comp in comps)
        {
            if (comp.gameObject.GetInstanceID() == GetInstanceID())
                continue;
            objects.Add((comp, comp.anchoredPosition));
        }
    }

    private void OnDisable()
    {
        foreach (var item in objects)
        {
            item.Item1.anchoredPosition = item.Item2;
        }
        objects.Clear();
    }

    private void Invalidate()
    {
        var offset = new Vector2();
        if (pressed)
            offset = new Vector2(0, PressedBent);
        else if (selected || entered)
            offset = new Vector2(0, SelectedBent); ;

        foreach (var item in objects) item.Item1.anchoredPosition = item.Item2 - offset;
    }
}