using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterCreationSliderUI : MonoBehaviour
{
    public Mode mode;
    public HumanCharacter character;

    public Slider slider;

    private void Awake()
    {
        slider.onValueChanged.AddListener(SetValue);
    }

    private void OnEnable()
    {
        Reload();
    }

    private void OnValidate()
    {
        Reload();
    }

    private void Reload()
    {
        slider.wholeNumbers = true;
        slider.minValue = 0;

        var type = mode.ToString();
        if (character == null)
        {
            slider.maxValue = 0;
        }
        else if (type.Contains("Color"))
        {
            if (type.Contains("Hair"))
                slider.maxValue = character.prefabs.hairColors.Length - 1;
            else
                slider.maxValue = character.prefabs.bodyColors.Length - 1;
        }
        else
        {
            var prefix = character.male ? "male" : "female";
            slider.maxValue = character.prefabs.Prefabs[prefix + type.ToString()].Count - 1;

            if (type.Contains("Hair"))
                slider.maxValue += 1;
        }
    }

    private void SetValue(float value)
    {
        int v = (int)value;
        switch (mode)
        {
            case Mode.Hair:
                character.hair = v;
                break;

            case Mode.Beard:
                character.beard = v;
                break;

            case Mode.Body:
                character.body = v;
                break;

            case Mode.Legs:
                character.legs = v;
                break;

            case Mode.Head:
                character.head = v;
                break;

            case Mode.HairColor:
                character.hairColor = v;
                break;

            case Mode.BodyColor:
                character.bodyColor = v;
                break;
        }

        switch (mode)
        {
            case Mode.Hair:
            case Mode.Beard:
            case Mode.Body:
            case Mode.Legs:
            case Mode.Head:
                character.InvalidateBody();
                character.InvalidateColors();
                character.SyncAnimation();
                break;

            case Mode.HairColor:
            case Mode.BodyColor:
                character.InvalidateColors();
                break;
        }
    }

    public enum Mode
    {
        Hair, Beard, Body, Legs, Head, HairColor, BodyColor
    }
}