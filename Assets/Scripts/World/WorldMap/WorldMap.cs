using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WorldGenerator;

public class WorldMap : MonoBehaviour
{
    public int Seed = 0;
    public SizeSettings SizeSettings;
    public IslandHeightSetterSettings IslandSettings;
    public TemperatureSetterSettings TemperatureSettings;
    public RiverSettings RiverSettings;
    public MoistureSetterSettings MoistureSettings;
    public BiomeSetterSettings BiomeSettings;
    public StructureSetterSettings StructureSettings;

    public List<WorldStructures.Region> Regions = new List<WorldStructures.Region>();
    public List<WorldStructures.Serializable.Region> regions;

    private CancellationTokenSource cancellationTokenSource;
    private Task generateJob;

    public int Hash = 0;

    private void OnEnable()
    {
        Reload();
    }

    private async Task GenerateMap(CancellationToken cancellationToken = new CancellationToken())
    {
        var world = new VornoiRegionCreator();
        world.Settings = SizeSettings;
        world.Seed = Seed;

        var islands = new IslandHeightSetter();
        islands.Seed = Seed;
        islands.Settings = IslandSettings;

        var rivers = new RiverCreator();
        rivers.Seed = Seed;
        rivers.Settings = RiverSettings;

        var temperature = new TemperatureSetter();
        temperature.Settings = TemperatureSettings;

        var moisture = new MoistureSetter();
        moisture.Settings = MoistureSettings;

        var biome = new BiomeSetter();
        biome.Settings = BiomeSettings;

        var structure = new StructureSetter();
        structure.Settings = StructureSettings;

        var regions = await world.Generate();
        regions = await islands.GenerateIsland(regions, cancellationToken);
        regions = await rivers.GenerateRivers(regions, cancellationToken);
        regions = await temperature.CalculateTemperature(regions, SizeSettings.Size.y, cancellationToken);
        regions = await moisture.CalculateMoisture(regions, cancellationToken);
        regions = await biome.CalculateBiome(regions, cancellationToken);
        regions = await structure.GenerateStructures(regions, cancellationToken);

        Regions = regions;
        this.regions = WorldStructures.Serializable.Region.Serialize(Regions);
        Hash = GetHashCode();
    }

    private async void Reload()
    {
        if (generateJob != null && !generateJob.IsCompleted)
        {
            cancellationTokenSource.Cancel();
            try
            {
                await generateJob;
            }
            catch (OperationCanceledException e)
            {
                //Debug.LogError(e);
            }
        }
        cancellationTokenSource = new CancellationTokenSource();
        generateJob = GenerateMap(cancellationTokenSource.Token);
    }

    private void OnValidate()
    {
        Reload();
    }

    public override bool Equals(object obj)
    {
        return obj is WorldMap map &&
               base.Equals(obj) &&
               EqualityComparer<List<WorldStructures.Serializable.Region>>.Default.Equals(regions, map.regions);
    }

    public override int GetHashCode()
    {
        var hashCode = 380617376;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<List<WorldStructures.Serializable.Region>>.Default.GetHashCode(regions);
        return hashCode;
    }
}