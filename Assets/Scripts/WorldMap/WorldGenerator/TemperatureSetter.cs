using System.Collections.Generic;
using WorldStructures;

namespace WorldGenerator
{
    [System.Serializable]
    public struct TemperatureSetterSettings
    {
        [UnityEngine.Range(0, 1)]
        public float TemperatureNorth;

        [UnityEngine.Range(0, 1)]
        public float TemperatureSouth;
    }

    public class TemperatureSetter
    {
        public TemperatureSetterSettings Settings;

        public void CalculateTemperature(ref List<Region> regions, int MapHeight)
        {
            float distance = (Settings.TemperatureNorth - Settings.TemperatureSouth);
            foreach (var region in regions)
            {
                float decreaseTemperature = region.Height / 8f;
                float factor = (region.Position.y / (float)MapHeight);
                region.Temperature = Settings.TemperatureSouth + (distance * factor) - decreaseTemperature;
            }
        }
    }
}