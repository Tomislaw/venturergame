using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        //ScreenOptions.Apply();
        Unroll();
    }

    public int rolloutSize = 350;
    public int height = 100;
    public float unrollSpeed = 1;
    public float rollSpeed = 1;

    public float hideDistance = -1000;
    public float hideSpeed = 1;

    public LTDescr Unroll()
    {
        var rect = GetComponent<RectTransform>();
        LeanTween.move(rect, rect.anchoredPosition + new Vector2(0, -rolloutSize), unrollSpeed).setEase(LeanTweenType.easeInOutCubic);
        return LeanTween.size(rect, rect.sizeDelta + new Vector2(0, rolloutSize), unrollSpeed).setEase(LeanTweenType.easeInOutCubic);
    }

    public LTDescr Roll()
    {
        var rect = GetComponent<RectTransform>();
        LeanTween.move(rect, rect.anchoredPosition + new Vector2(0, rolloutSize), rollSpeed).setEase(LeanTweenType.easeInOutCubic);
        return LeanTween.size(rect, new Vector2(rect.sizeDelta.x, height), rollSpeed).setEase(LeanTweenType.easeInOutCubic);
    }

    public LTDescr HideOnBottom()
    {
        var rect = GetComponent<RectTransform>();
        LeanTween.move(rect, rect.anchoredPosition + new Vector2(0, hideDistance), hideSpeed).setEase(LeanTweenType.easeInOutCubic);
        return LeanTween.rotate(rect, new Vector2(30, hideDistance), hideSpeed).setEase(LeanTweenType.easeInOutCubic);
    }

    public LTDescr ShowFromBottom()
    {
        var rect = GetComponent<RectTransform>();
        LeanTween.move(rect, rect.anchoredPosition - new Vector2(0, hideDistance), hideSpeed).setEase(LeanTweenType.easeInOutCubic);
        return LeanTween.rotate(rect, new Vector2(0, hideDistance), hideSpeed).setEase(LeanTweenType.easeInOutCubic);
    }

    public void Show(bool show)
    {
        if (show)
        {
            Unroll().setOnComplete(() => { ShowFromBottom(); });
        }
        else
        {
            Roll().setOnComplete(() => { HideOnBottom(); });
        }
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