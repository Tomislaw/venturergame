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
    public float timeUntilTextIsShown = 1;
    private AsyncOperation operation;
    private float waitingTime;

    public void LoadScene(string name)
    {
        gameObject.SetActive(true);
        waitingTime = 0;
        DontDestroyOnLoad(gameObject);

        text.text = "";
        background.color = new Color(0, 0, 0, 0);

        var seq = LeanTween.sequence();
        seq.append(LeanTween
            .value(background.gameObject, a => background.color = a, new Color(0, 0, 0, 0), Color.black, fadeTime)
            .setEase(LeanTweenType.linear));
        seq.append(() => { operation = SceneManager.LoadSceneAsync(name); });
    }

    private void FixedUpdate()
    {
        if (operation != null)
        {
            waitingTime += Time.fixedDeltaTime;

            if (waitingTime > timeUntilTextIsShown)
            {
                string dots = "";
                for (int i = 0; i < operation.progress * 5; i++)
                    dots += '.';
                text.text = "Loading" + dots;
            }

            if (operation.isDone)
            {
                operation = null;
                var seq = LeanTween.sequence();

                LeanTween.value(background.gameObject, a => background.color = a, Color.black, new Color(0, 0, 0, 0), fadeTime * 0.99f);
                seq.append(LeanTween
                    .value(text.gameObject, a => text.color = a, Color.white, new Color(0, 0, 0, 0), fadeTime)
                    .setEase(LeanTweenType.linear));
                seq.append(() => Destroy(gameObject));
            }
        }
    }
}