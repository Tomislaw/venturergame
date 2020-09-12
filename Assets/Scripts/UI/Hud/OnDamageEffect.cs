using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Damageable))]
public class OnDamageEffect : MonoBehaviour, Damageable.OnDamage
{
    public float effectTime = 0.2f;
    public Color color = Color.red;

    private float timeLeft = 0;
    private Coroutine coroutine;

    private IEnumerator Flick()
    {
        var mainSprite = GetComponent<SpriteAnimator>();
        var sprites = GetComponentsInChildren<SpriteAnimator>();

        timeLeft = effectTime;
        while (timeLeft > 0)
        {
            if (mainSprite?.spriteRenderer != null)
                mainSprite.spriteRenderer.color = Color.Lerp(color, Color.white, 1 - timeLeft / effectTime);
            for (int i = 0; i < sprites.Length; i++)
                if (sprites[i]?.spriteRenderer != null)
                    sprites[i].spriteRenderer.color = Color.Lerp(color, Color.white, 1 - timeLeft / effectTime);
            timeLeft -= Time.deltaTime;

            yield return 0;
        }

        if (mainSprite?.spriteRenderer != null)
            mainSprite.spriteRenderer.color = Color.white;
        for (int i = 0; i < sprites.Length; i++)
            if (sprites[i]?.spriteRenderer != null)
                sprites[i].spriteRenderer.color = Color.white;

        yield return 0;
    }

    public void OnDamage(Damageable.DamageData damage)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        StartCoroutine(Flick());
    }
}