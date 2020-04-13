using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCharacter : MonoBehaviour
{
    public List<SpriteAnimator> femaleHeads;
    public List<SpriteAnimator> maleHeads;
    public List<SpriteAnimator> femaleBodies;
    public List<SpriteAnimator> maleBodies;

    public bool male = false;
    public int body = 0;
    public int head = 0;

    private SpriteAnimator headSprite;
    private SpriteAnimator bodySprite;

    private SpriteAnimator helmet;
    private SpriteAnimator armor;
    private SpriteAnimator pants;
    private SpriteAnimator boots;

    public void ReloadCharacterSprites()
    {
        string headAnimation = "idle";
        string bodyAnimation = "idle";

        Transform _head = transform.Find("head");
        Transform _body = transform.Find("body");

        if (_head != null)
        {
            headAnimation = _head.GetComponent<SpriteAnimator>().GetAnimation().name;

            if (Application.isEditor)
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(_head.gameObject);
                };
            else
                Destroy(_head.gameObject);
        }
        if (_body != null)
        {
            bodyAnimation = _body.GetComponent<SpriteAnimator>().GetAnimation().name;

            if (Application.isEditor)
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(_body.gameObject);
                };
            else
                Destroy(_body.gameObject);
        }

        List<SpriteAnimator> heads = femaleHeads;
        List<SpriteAnimator> bodies = femaleBodies;
        if (male)
        {
            heads = maleHeads;
            bodies = maleBodies;
        }

        if (body < bodies.Count)
        {
            SpriteAnimator b = Instantiate(bodies[body]);
            b.name = "body";
            b.transform.parent = transform;
            b.transform.localPosition = new Vector2();
            bodySprite = b;
        }

        if (head < heads.Count)
        {
            SpriteAnimator h = Instantiate(heads[body]);
            h.name = "head";
            h.transform.parent = transform;
            h.transform.localPosition = new Vector2();
            headSprite = h;
        }

        if (headSprite != null)
            headSprite.SetAnimation(headAnimation);
        if (bodySprite != null)
            bodySprite.SetAnimation(bodyAnimation);
    }

    public void SetAnimation(string animation)
    {
        if (headSprite != null)
            headSprite.SetAnimation(animation);
        if (bodySprite != null)
            bodySprite.SetAnimation(animation);

        if (helmet != null)
            helmet.SetAnimation(animation);
        if (armor != null)
            armor.SetAnimation(animation);
        if (pants != null)
            pants.SetAnimation(animation);
        if (boots != null)
            boots.SetAnimation(animation);
    }

    // Start is called before the first frame update
    private void Start()
    {
        ReloadCharacterSprites();
    }

    private void OnValidate()
    {
        ReloadCharacterSprites();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}