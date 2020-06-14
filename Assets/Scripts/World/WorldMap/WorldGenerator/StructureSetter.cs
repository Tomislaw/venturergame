using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WorldStructures;

namespace WorldGenerator
{
    [System.Serializable]
    public struct StructureSetterSettings
    {
        public int SaltDepositCount;
        public int IronDepositCount;
        public int TownCount;
        public int VillageCount;
    }

    public class StructureSetter
    {
        public StructureSetterSettings Settings;
        public int Seed = 0;
        private System.Random random;

        public Task<List<Region>> GenerateStructures(List<Region> regions, CancellationToken cancellationToken = new CancellationToken())
        {
            random = new System.Random(Seed);

            Task<List<Region>> task = null;
            task = Task.Run(() =>
            {
                GenerateDeposits(ref regions, cancellationToken);
                GenerateVillages(ref regions, cancellationToken);
                GenerateTowns(ref regions, cancellationToken);

                return regions;
            });
            return task;
        }

        private void GenerateDeposits(ref List<Region> regions, CancellationToken cancellationToken)
        {
            var validIronPositions = new List<(Region, float)>();
            foreach (var region in regions)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (region.biome != Biome.Mountians)
                    continue;

                validIronPositions.Add((region, (float)random.NextDouble()));
            }
            foreach (var r in validIronPositions.OrderByDescending(it => it.Item2).Take(Settings.IronDepositCount))
            {
                cancellationToken.ThrowIfCancellationRequested();

                r.Item1.structure = Structure.IronDeposit;
            }
            var validSaltPositions = new List<(Region, float)>();
            foreach (var region in regions)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (region.biome != Biome.TemperateDesert && region.biome != Biome.SubtropicalDesert)
                    continue;

                validSaltPositions.Add((region, (float)random.NextDouble()));
            }
            foreach (var r in validSaltPositions.OrderByDescending(it => it.Item2).Take(Settings.SaltDepositCount))
            {
                cancellationToken.ThrowIfCancellationRequested();

                r.Item1.structure = Structure.SaltDeposit;
            }
        }

        private void GenerateVillages(ref List<Region> regions, CancellationToken cancellationToken)
        {
            var validVillagePositions = new List<(Region, float)>();
            foreach (var region in regions)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (region.Corner || region.Water || region.structure != Structure.None)
                    continue;

                float howGoodIsVillagePlacement = 0;

                howGoodIsVillagePlacement += region.Rivers.Count;
                howGoodIsVillagePlacement += (1f - Math.Abs(region.Temperature * 2 - 1)) * 5;
                howGoodIsVillagePlacement += (1f - Math.Abs(region.Moisture * 2 - 1)) * 5;
                if (region.biome == Biome.Shrubland)
                    howGoodIsVillagePlacement += 3;
                if (region.biome == Biome.DeciduousForest || region.biome == Biome.MixedForests)
                    howGoodIsVillagePlacement += 1;

                foreach (var neighbour in region.Neighbors)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (neighbour.structure == Structure.IronDeposit)
                        howGoodIsVillagePlacement += 8;

                    if (neighbour.structure == Structure.SaltDeposit)
                        howGoodIsVillagePlacement += 6;

                    if (neighbour.biome == Biome.Lake)
                        howGoodIsVillagePlacement += 4;
                }

                validVillagePositions.Add((region, howGoodIsVillagePlacement));
            }

            var villages = new List<Region>();
            int villagesLeft = Settings.VillageCount;

            while (validVillagePositions.Count > 0 && villagesLeft > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var villageRegion = validVillagePositions.Aggregate(
                    (agg, next) => DistanceModifier(ref villages, next, 3) > DistanceModifier(ref villages, agg, 3)
                    ? next : agg);

                villageRegion.Item1.structure = Structure.Village;

                foreach (var neighbour in villageRegion.Item1.Neighbors)
                {
                    validVillagePositions.RemoveAll(it => it.Item1 == neighbour);
                }

                validVillagePositions.Remove(villageRegion);
                villagesLeft--;
                villages.Add(villageRegion.Item1);
            }
        }

        private void GenerateTowns(ref List<Region> regions, CancellationToken cancellationToken)
        {
            var validTownPositions = new List<(Region, float)>();

            var villages = regions.FindAll(it => it.structure == Structure.Village);

            foreach (var region in regions)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (region.Corner || region.Water || region.structure != Structure.None)
                    continue;

                foreach (var neighbour in region.Neighbors)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (neighbour.structure == Structure.Village)
                        continue;
                }

                float howGoodIsTownPlacement = 0;

                foreach (var village in villages)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (IsRegionsNearby(region, village))
                        howGoodIsTownPlacement -= 50;
                    else if (IsRegionsVeryClose(region, village))
                        howGoodIsTownPlacement += 30;
                    else if (IsRegionsClose(region, village))
                        howGoodIsTownPlacement += 10;
                    else if (IsRegionsModerateClose(region, village))
                        howGoodIsTownPlacement += 5;
                }
                if (region.Coast)
                    howGoodIsTownPlacement += 5;

                howGoodIsTownPlacement += region.Rivers.Count;
                howGoodIsTownPlacement += (1f - Math.Abs(region.Temperature * 2 - 1)) * 5;
                howGoodIsTownPlacement += (1f - Math.Abs(region.Moisture * 2 - 1)) * 5;
                if (region.biome == Biome.Shrubland)
                    howGoodIsTownPlacement += 3;
                if (region.biome == Biome.DeciduousForest || region.biome == Biome.MixedForests)
                    howGoodIsTownPlacement += 1;

                foreach (var neighbour in region.Neighbors)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (neighbour.structure == Structure.IronDeposit)
                        howGoodIsTownPlacement -= 6;

                    if (neighbour.structure == Structure.SaltDeposit)
                        howGoodIsTownPlacement -= 5;
                }

                validTownPositions.Add((region, howGoodIsTownPlacement));
            }

            int townsLeft = Settings.TownCount;
            var towns = new List<Region>();

            while (validTownPositions.Count > 0 && townsLeft > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var townRegion = validTownPositions.Aggregate(
                    (agg, next) => DistanceModifier(ref towns, next) > DistanceModifier(ref towns, agg, 10)
                    ? next : agg);

                townRegion.Item1.structure = Structure.Town;

                foreach (var neighbour in townRegion.Item1.Neighbors)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    validTownPositions.RemoveAll(it => it.Item1 == neighbour);
                }

                validTownPositions.Remove(townRegion);
                towns.Add(townRegion.Item1);
                townsLeft--;
            }
        }

        private float DistanceModifier(ref List<Region> structures, (Region, float) region, int value = 10)
        {
            float newValue = region.Item2;
            foreach (var structure in structures)
            {
                if (IsRegionsClose(structure, region.Item1))
                    newValue -= value;
                if (IsRegionsVeryClose(structure, region.Item1))
                    newValue -= value;
                if (IsRegionsModerateClose(structure, region.Item1))
                    newValue -= value;
            }
            return newValue;
        }

        private bool IsRegionsNearby(Region a, Region b)
        {
            var distance = Vector2.Distance(a.Position, b.Position);
            return distance < 40;
        }

        private bool IsRegionsVeryClose(Region a, Region b)
        {
            var distance = Vector2.Distance(a.Position, b.Position);
            return distance < 80;
        }

        private bool IsRegionsClose(Region a, Region b)
        {
            var distance = Vector2.Distance(a.Position, b.Position);
            return distance < 400;
        }

        private bool IsRegionsModerateClose(Region a, Region b)
        {
            var distance = Vector2.Distance(a.Position, b.Position);
            return distance < 1000;
        }
    }
}