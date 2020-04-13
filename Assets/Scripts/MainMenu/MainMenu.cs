using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        ScreenOptions.Apply();
        Unroll();
    }

    private int rolloutSize = 350;

    public LTDescr Unroll()
    {
        var rect = GetComponent<RectTransform>();
        LeanTween.move(rect, rect.anchoredPosition + new Vector2(0, -rolloutSize / 2), 1).setEase(LeanTweenType.easeInOutCubic);
        return LeanTween.size(rect, rect.sizeDelta + new Vector2(0, rolloutSize), 1).setEase(LeanTweenType.easeInOutCubic);
    }

    public LTDescr Roll()
    {
        var rect = GetComponent<RectTransform>();
        LeanTween.move(rect, rect.anchoredPosition + new Vector2(0, rolloutSize / 2), 0.3f).setEase(LeanTweenType.easeInOutCubic);
        return LeanTween.size(rect, new Vector2(rect.sizeDelta.x, 65), 0.3f).setEase(LeanTweenType.easeInOutCubic);
    }

    public void ExitGame()
    {
        Roll().setOnComplete(() => Application.Quit());
    }

    public void Continue()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}