using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterCreationSliderUI : MonoBehaviour
{
    public Mode mode;
    public HumanModel character;

    public Slider slider;

    private HumanModelPrefabs prefabs { get => character.male ? character.malePrefabs : character.femalePrefabs; }

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
                slider.maxValue = prefabs.hairColors.Length - 1;
            else
                slider.maxValue = prefabs.bodyColors.Length - 1;
        }
        else
        {
            switch (mode)
            {
                case Mode.Hair:
                    slider.maxValue = prefabs.Hairs.Count;
                    break;

                case Mode.Beard:
                    slider.maxValue = prefabs.Beards.Count - 1;
                    break;

                case Mode.Body:
                    slider.maxValue = prefabs.Bodies.Count - 1;
                    break;

                case Mode.Legs:
                    slider.maxValue = prefabs.Legs.Count - 1;
                    break;

                case Mode.Head:
                    slider.maxValue = prefabs.Heads.Count - 1;
                    break;

                case Mode.HairColor:
                    slider.maxValue = prefabs.hairColors.Length - 1;
                    break;

                case Mode.BodyColor:
                    slider.maxValue = prefabs.bodyColors.Length - 1;
                    break;
            }
        }
    }

    private void SetValue(float value)
    {
        int v = (int)value;
        switch (mode)
        {
            case Mode.Hair:
                character.hairType = v;
                break;

            case Mode.Beard:
                character.beardType = v;
                break;

            case Mode.Body:
                character.bodyType = v;
                break;

            case Mode.Legs:
                character.legsType = v;
                break;

            case Mode.Head:
                character.headType = v;
                break;

            case Mode.HairColor:
                character.hairColor = v;
                break;

            case Mode.BodyColor:
                character.bodyColor = v;

                break;
        }
        character.Invalidate();
    }

    public enum Mode
    {
        Hair, Beard, Body, Legs, Head, HairColor, BodyColor
    }
}