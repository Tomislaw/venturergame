using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface Animator
{
    string Animation { get; set; }
    int AnimationFrame { get; set; }
    float TimeForAnimation { get; set; }

    void SetAnimation(string animation, bool restartIfSame);

    void Sync(Animator animator);

    GameObject gameObject { get; }
}