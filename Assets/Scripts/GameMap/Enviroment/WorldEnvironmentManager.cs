using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WorldEnvironmentManager : MonoBehaviour
{
    public World world;

    public MoonAndSun moonAndSun;
    public Background background;
    public LightColorOscilator globalLight;
    public LightColorOscilator skyboxLight;

    private void Update()
    {
        float value = world.environment.normalizedDayTime;
        moonAndSun.value = value;
        background.value = value;
        globalLight.value = value;
        skyboxLight.value = value;
    }
}