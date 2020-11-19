using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScreenUI : MonoBehaviour
{
    public Image background;
    public TMP_Text text;
    public float fadeTime = 1;

    private AsyncOperation operation;

    public void LoadScene(string name)
    {
        gameObject.SetActive(true);

        DontDestroyOnLoad(gameObject);
        text.color = new Color(0, 0, 0, 0);
        background.color = new Color(0, 0, 0, 0);
        var seq = LeanTween.sequence();
        seq.append(LeanTween
            .value(background.gameObject, a => background.color = a, new Color(0, 0, 0, 0), Color.black, fadeTime / 2)
            .setEase(LeanTweenType.linear));
        seq.append(LeanTween
            .value(text.gameObject, a => text.color = a, new Color(0, 0, 0, 0), Color.white, fadeTime / 2)
            .setEase(LeanTweenType.linear));
        seq.append(() => { operation = SceneManager.LoadSceneAsync(name); });
    }

    private void FixedUpdate()
    {
        if (operation != null)
        {
            string dots = "";
            for (int i = 0; i < operation.progress * 5; i++)
                dots += '.';
            text.SetText("Loading" + dots);

            if (operation.isDone)
            {
                operation = null;
                var seq = LeanTween.sequence();
                seq.append(LeanTween
                    .value(text.gameObject, a => text.color = a, Color.white, new Color(0, 0, 0, 0), fadeTime / 2)
                    .setEase(LeanTweenType.linear));
                seq.append(LeanTween
                    .value(background.gameObject, a => background.color = a, Color.black, new Color(0, 0, 0, 0), fadeTime / 2)
                    .setEase(LeanTweenType.linear));
                seq.append(() => Destroy(gameObject));
            }
        }
    }
}