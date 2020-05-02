using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CharacterMovementController))]
public class HumanCharacter : MonoBehaviour
{
    public bool male = false;

    private Dictionary<string, SimpleAnimator> sprites = new Dictionary<string, SimpleAnimator>();

    public void Equip(Equipment equipment)
    {
        Unequip(equipment.type);

        GameObject animator = male ? equipment.maleSpriteSheet : equipment.femaleSpriteSheet;
        if (animator != null && animator.GetComponent<SimpleAnimator>() != null)
        {
            var item = Instantiate(animator);
            item.name = equipment.type.ToString().ToLower();
            item.transform.parent = transform;
            item.transform.localPosition = new Vector2();
            item.transform.localScale = new Vector3(1, 1, 1);
            sprites[item.name] = item.GetComponent<SimpleAnimator>();
        }

        StartCoroutine(SyncAnimation());
    }

    public void Unequip(Equipment.Type type)
    {
        var name = type.ToString().ToLower();

        sprites.Remove(name);
        Transform item = transform.Find(name);
        if (item != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(item.gameObject);
#else
            Destroy(item.gameObject);
#endif
        }
    }

    private IEnumerator SyncAnimation()
    {
        yield return null;
        SimpleAnimator first = null;
        foreach (var item in sprites)
        {
            if (first == null)
                first = item.Value;
            else
            {
                item.Value.Animation = first.Animation;
                item.Value.Sync(first);
            }
        }
        yield return null;
    }

    public void SetAnimation(string animation)
    {
        foreach (var item in sprites)
            item.Value.Animation = animation;
    }

    // Start is called before the first frame update
    private void Start()
    {
        var components = GetComponentsInChildren<SimpleAnimator>();
        foreach (var c in components)
            sprites.Add(c.name, c);
    }

    // Update is called once per frame
    private void Update()
    {
        var controller = GetComponent<CharacterMovementController>();
        var attackController = GetComponent<CharacterBasicAttackController>();
        var inventoryController = GetComponent<CharacterInventoryController>();
        var blockController = GetComponent<CharacterBlockComponent>();

        if (controller.IsRunning)
        {
            if (inventoryController && inventoryController.GetEquippedWeapon())
                SetAnimation("run");
            else
                SetAnimation("runww");
            attackController.CancelAttack();
            blockController.StopBlocking();
        }
        else if (controller.IsWalking)
        {
            SetAnimation("walk");
            attackController.CancelAttack();
            blockController.StopBlocking();
        }
        else if (attackController.IsStartedAttacking)
        {
            SetAnimation("attack" + Random.Range(1, 3).ToString());
        }
        else if (attackController.IsAttacking)
        {
        }
        else if (blockController.IsPreparingToBlock)
        {
            if (inventoryController)
            {
                if (inventoryController.GetEquippedOffhand())
                    SetAnimation("block1");
                else if (inventoryController.GetEquippedWeapon())
                    SetAnimation("block2");
                else
                    SetAnimation("block1");
            }
            else
            {
                SetAnimation("block1");
            }
        }
        else if (blockController.IsBlocking)
        {
        }
        else
        {
            SetAnimation("idle");
        }

        if (Input.GetKey(KeyCode.Keypad0))
            SetAnimation("death");
        if (Input.GetKey(KeyCode.Keypad1))
            SetAnimation("attack1");
        if (Input.GetKey(KeyCode.Keypad2))
            SetAnimation("attack2");
        if (Input.GetKey(KeyCode.Keypad3))
            SetAnimation("block1");
        if (Input.GetKey(KeyCode.Keypad4))
            SetAnimation("block2");
        if (Input.GetKey(KeyCode.Keypad5))
            SetAnimation("run");
    }
}