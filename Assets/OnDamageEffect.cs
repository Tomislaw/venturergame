using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Damageable))]
public class OnDamageEffect : MonoBehaviour
{
    private Damageable damageable;

    public float effectTime = 0.2f;
    public Color color;

    private float timeLeft = 0;
    private Coroutine coroutine;

    private void OnEnable()
    {
        damageable = GetComponent<Damageable>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (damageable.HealthChange < 0)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            StartCoroutine(Flick());
        }
    }

    private IEnumerator Flick()
    {
        var mainSprite = GetComponent<SpriteAnimator>();
        var sprites = GetComponentsInChildren<SpriteAnimator>();

        timeLeft = effectTime;
        while (timeLeft > 0)
        {
            mainSprite.spriteRenderer.color = Color.Lerp(color, Color.white, 1 - timeLeft / effectTime);
            for (int i = 0; i < sprites.Length; i++)
                sprites[i].spriteRenderer.color = Color.Lerp(color, Color.white, 1 - timeLeft / effectTime);
            timeLeft -= Time.deltaTime;

            yield return 0;
        }

        mainSprite.spriteRenderer.color = Color.white;
        for (int i = 0; i < sprites.Length; i++)
            sprites[i].spriteRenderer.color = Color.white;

        yield return 0;
    }
}