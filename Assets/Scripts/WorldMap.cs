using System.Collections.Generic;
using UnityEngine;
using VoronoiLib.Structures;
using WorldGenerator;
using WorldStructures;

public class WorldMap : MonoBehaviour
{
    public int Seed = 0;
    public WorldGenerator.SizeSettings SizeSettings;
    public WorldGenerator.IslandHeightSetterSettings IslandSettings;
    public WorldGenerator.TemperatureSetterSettings TemperatureSettings;
    public WorldGenerator.RiverSettings RiverSettings;
    public WorldGenerator.MoistureSetterSettings MoistureSettings;
    public WorldGenerator.BiomeSetterSettings BiomeSettings;
    public List<Region> Regions = new List<Region>();

    private void Awake()
    {
        Reload();
    }

    private void Reload()
    {
        var world = new WorldGenerator.VornoiRegionCreator();
        world.Settings = SizeSettings;
        world.Seed = Seed;

        var islands = new WorldGenerator.IslandHeightSetter();
        islands.Seed = Seed;
        islands.Settings = IslandSettings;

        var rivers = new WorldGenerator.RiverCreator();
        rivers.Seed = Seed;
        rivers.Settings = RiverSettings;

        var temperature = new WorldGenerator.TemperatureSetter();
        temperature.Settings = TemperatureSettings;

        var moisture = new WorldGenerator.MoistureSetter();
        moisture.Settings = MoistureSettings;

        var biome = new WorldGenerator.BiomeSetter();
        biome.Settings = BiomeSettings;

        Regions = world.Generate();
        islands.GenerateIsland(ref Regions);
        rivers.GenerateRivers(ref Regions);
        temperature.CalculateTemperature(ref Regions, SizeSettings.Size.y);
        moisture.CalculateMoisture(ref Regions);
        biome.CalculateBiome(ref Regions);
    }

    private void OnValidate()
    {
        Reload();
    }

    public override bool Equals(object obj)
    {
        return obj is WorldMap map &&
               base.Equals(obj) &&
               Seed == map.Seed &&
               EqualityComparer<SizeSettings>.Default.Equals(SizeSettings, map.SizeSettings) &&
               EqualityComparer<IslandHeightSetterSettings>.Default.Equals(IslandSettings, map.IslandSettings) &&
               EqualityComparer<TemperatureSetterSettings>.Default.Equals(TemperatureSettings, map.TemperatureSettings) &&
               EqualityComparer<RiverSettings>.Default.Equals(RiverSettings, map.RiverSettings) &&
               EqualityComparer<BiomeSetterSettings>.Default.Equals(BiomeSettings, map.BiomeSettings) &&
               EqualityComparer<MoistureSetterSettings>.Default.Equals(MoistureSettings, map.MoistureSettings);
    }

    public override int GetHashCode()
    {
        var hashCode = -1282697966;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + Seed.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<SizeSettings>.Default.GetHashCode(SizeSettings);
        hashCode = hashCode * -1521134295 + EqualityComparer<IslandHeightSetterSettings>.Default.GetHashCode(IslandSettings);
        hashCode = hashCode * -1521134295 + EqualityComparer<TemperatureSetterSettings>.Default.GetHashCode(TemperatureSettings);
        hashCode = hashCode * -1521134295 + EqualityComparer<RiverSettings>.Default.GetHashCode(RiverSettings);
        hashCode = hashCode * -1521134295 + EqualityComparer<MoistureSetterSettings>.Default.GetHashCode(MoistureSettings);
        hashCode = hashCode * -1521134295 + EqualityComparer<BiomeSetterSettings>.Default.GetHashCode(BiomeSettings);
        return hashCode;
    }
}