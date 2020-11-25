using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WorldEnvironmentManager : MonoBehaviour
{
    public WorldEnvironment world;

    public MoonAndSun moonAndSun;
    public Background background;

    public List<ObjectColorOscilator> colorOscilators;

    private void Update()
    {
        float value = world.normalizedDayTime;
        moonAndSun.value = value;
        background.value = value;
        foreach (var oscilator in colorOscilators)
            oscilator.value = value;
    }
}