using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class TabGui : MonoBehaviour
{
    public string TabName;

    public bool IsSelected = false;
    public float rolloutDistance = 10;
    public float rolloutTime = 0.1f;

    public TabsGui tabsGui;

    public void Select(bool isSelected)
    {
        if (isSelected)
        {
            if (IsSelected)
                return;

            IsSelected = isSelected;

            var rect = GetComponent<RectTransform>();

            LeanTween.moveX(rect, rect.anchoredPosition.x - rolloutDistance, rolloutTime);

            if (tabsGui != null)
                tabsGui.SelectTab(this);
        }
        else
        {
            if (!IsSelected)
                return;

            IsSelected = isSelected;

            var rect = GetComponent<RectTransform>();

            LeanTween.moveX(rect, rect.anchoredPosition.x + rolloutDistance, rolloutTime);
        }
    }

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}