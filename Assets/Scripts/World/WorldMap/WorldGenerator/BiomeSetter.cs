using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorldStructures;

namespace WorldGenerator
{
    [System.Serializable]
    public struct BiomeSetterSettings
    {
        public int MountianHeight;
    }

    public class BiomeSetter
    {
        public BiomeSetterSettings Settings;

        public Task<List<Region>> CalculateBiome(List<Region> regions, CancellationToken cancellationToken = new CancellationToken())
        {
            Task<List<Region>> task = null;
            task = Task.Run(() =>
            {
                foreach (var region in regions)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (region.Water)
                    {
                        if (region.ConnectedToOcean)
                            region.biome = region.Temperature < 0.1 ? Biome.Ice : Biome.Ocean;
                        else
                            region.biome = region.Temperature < 0.25 ? Biome.Ice : Biome.Lake;
                    }
                    else if (region.Height >= Settings.MountianHeight)
                    {
                        region.biome = Biome.Mountians;
                    }
                    else
                    {
                        if (region.Height == 0 && region.Coast)
                        {
                            if (region.Temperature < 0.15)
                                region.biome = Biome.Snow;
                            else
                                region.biome = Biome.Beach;
                        }
                        else if (region.Temperature < 0.1)
                        {
                            if (region.Moisture < 0.1)
                                region.biome = Biome.Bare;
                            else
                                region.biome = Biome.Snow;
                        }
                        else if (region.Temperature < 0.2)
                        {
                            if (region.Moisture < 0.1)
                                region.biome = Biome.Bare;
                            else if (region.Moisture < 0.4)
                                region.biome = Biome.Snow;
                            else
                                region.biome = Biome.Tundra;
                        }
                        else if (region.Temperature < 0.3)
                        {
                            if (region.Moisture < 0.2)
                                region.biome = Biome.Bare;
                            else if (region.Moisture < 0.40)
                                region.biome = Biome.Tundra;
                            else if (region.Moisture < 0.60)
                                region.biome = Biome.Shrubland;
                            else
                                region.biome = Biome.Taiga;
                        }
                        else if (region.Temperature < 0.5)
                        {
                            if (region.Moisture < 0.3)
                                region.biome = Biome.Bare;
                            else if (region.Moisture < 0.55)
                                region.biome = Biome.Shrubland;
                            else
                                region.biome = Biome.MixedForests;
                        }
                        else if (region.Temperature < 0.7)
                        {
                            if (region.Moisture < 0.2)
                                region.biome = Biome.Bare;
                            else if (region.Moisture < 0.55)
                                region.biome = Biome.Shrubland;
                            else
                                region.biome = Biome.DeciduousForest;
                        }
                        else
                        {
                            if (region.Moisture < 0.25)
                                region.biome = Biome.SubtropicalDesert;
                            else if (region.Moisture < 0.55)
                                region.biome = Biome.TemperateDesert;
                            else
                                region.biome = Biome.TropicalRainForest;
                        }
                    }
                }
                return regions;
            });
            return task;
        }
    }
}