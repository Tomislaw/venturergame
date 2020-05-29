using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        public Task<List<Region>> CalculateTemperature(List<Region> regions, int MapHeight, CancellationToken cancellationToken = new CancellationToken())
        {
            Task<List<Region>> task = null;
            task = Task.Run(() =>
            {
                float distance = (Settings.TemperatureNorth - Settings.TemperatureSouth);
                foreach (var region in regions)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    float decreaseTemperature = region.Height / 8f;
                    float factor = (region.Position.y / (float)MapHeight);
                    region.Temperature = Settings.TemperatureSouth + (distance * factor) - decreaseTemperature;
                }
                return regions;
            });
            return task;
        }
    }
}