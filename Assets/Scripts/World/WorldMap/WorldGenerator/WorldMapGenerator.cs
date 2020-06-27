using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WorldGenerator;

public interface IWorldMap
{
    void SetRegions(List<WorldStructures.Serializable.Region> Regions);
}

public class WorldMapGenerator : MonoBehaviour
{
    public int Seed = 0;
    public SizeSettings SizeSettings;
    public IslandHeightSetterSettings IslandSettings;
    public TemperatureSetterSettings TemperatureSettings;
    public RiverSettings RiverSettings;
    public MoistureSetterSettings MoistureSettings;
    public BiomeSetterSettings BiomeSettings;
    public StructureSetterSettings StructureSettings;

    //public List<WorldStructures.Serializable.Region> Regions;

    private CancellationTokenSource cancellationTokenSource;
    private Task generateJob;

    public WorldMap WorldMap;

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

        var roads = new RoadSetter();

        var regions = await world.Generate();
        regions = await islands.GenerateIsland(regions, cancellationToken);
        regions = await rivers.GenerateRivers(regions, cancellationToken);
        regions = await temperature.CalculateTemperature(regions, SizeSettings.Size.y, cancellationToken);
        regions = await moisture.CalculateMoisture(regions, cancellationToken);
        regions = await biome.CalculateBiome(regions, cancellationToken);
        regions = await structure.GenerateStructures(regions, cancellationToken);
        regions = await structure.GenerateStructures(regions, cancellationToken);
        regions = await roads.GenerateInitialRoads(regions, cancellationToken);

        var serialized = WorldStructures.Serializable.Region.Serialize(regions);

        WorldMap.SetRegions(serialized);
        WorldMap.Size = SizeSettings.Size;
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
                Debug.Log("Generating world cancelled");
            }
        }
        cancellationTokenSource = new CancellationTokenSource();
        generateJob = GenerateMap(cancellationTokenSource.Token);
    }

    private void OnValidate()
    {
        Reload();
    }
}