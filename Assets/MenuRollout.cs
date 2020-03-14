using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRollout : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        var rect = GetComponent<RectTransform>();
        int size = 350;
        LeanTween.size(rect, rect.sizeDelta + new Vector2(0, size), 1).setEase(LeanTweenType.easeInOutCubic);
        LeanTween.move(rect, rect.anchoredPosition + new Vector2(0, -size / 3), 1).setEase(LeanTweenType.easeInOutCubic);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}