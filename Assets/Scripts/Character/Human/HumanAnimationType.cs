﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SimpleAnimator))]
public class HumanAnimationType : MonoBehaviour
{
    public Type AnimationType;

    public enum Type
    {
        Main, Legs
    }
}