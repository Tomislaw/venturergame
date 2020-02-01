using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChunkGenerators
{
    public static class DecidousChunkGenerator
    {
        public static List<ChunkDecoratorData> GenerateForest()
        {
            List<ChunkDecoratorData> chunk = new List<ChunkDecoratorData>();

            int trees = 6;
            int saplings = 4;
            int bushes = 4;
            int shrubs = 10;
            int seed = 0;

            //Generate grass
            var rng = new System.Random(seed);
            for (int i = 0; i < Chunk.CHUNK_SIZE; i++)
            {
                ChunkDecoratorData grass;
                grass.name = "grass_center" + (i % 2 + 1);
                grass.properties = new Dictionary<string, object>();
                grass.properties.Add("seed", rng.Next());
                grass.properties.Add("position", new Vector3(i, 0, 0));
                chunk.Add(grass);
            }

            int previous = 0;
            //generate saplings
            previous = 0;
            for (int i = 0; i < saplings; i++)
            {
                previous = rng.NextExcluding(1, 5, previous);

                ChunkDecoratorData sapling;
                sapling.name = "deciduous_sapling" + previous;
                sapling.properties = new Dictionary<string, object>();
                sapling.properties.Add("position",
                    new Vector3(
                        Mathf.Lerp(0, Chunk.CHUNK_SIZE, (float)i / saplings),
                        0,
                        1));
                chunk.Add(sapling);
            }

            //generate shrubs
            previous = 0;
            for (int i = 0; i < shrubs; i++)
            {
                previous = rng.NextExcluding(1, 5, previous);

                ChunkDecoratorData bush;
                bush.name = "deciduous_shrub" + previous;
                bush.properties = new Dictionary<string, object>();
                bush.properties.Add("position",
                    new Vector3(
                    Mathf.Lerp(0, Chunk.CHUNK_SIZE, (float)i / shrubs),
                    0,
                    2));
                chunk.Add(bush);
            }

            //generate bush
            previous = 0;
            for (int i = 0; i < bushes; i++)
            {
                previous = rng.NextExcluding(1, 5, previous);

                ChunkDecoratorData bush;
                bush.name = "deciduous_bush" + previous;
                bush.properties = new Dictionary<string, object>();
                bush.properties.Add("position", new Vector3(
                    Mathf.Lerp(0, Chunk.CHUNK_SIZE, (float)i / bushes),
                    0,
                    3));
                chunk.Add(bush);
            }

            //generate trees
            previous = 0;
            for (int i = 0; i < trees; i++)
            {
                previous = rng.NextExcluding(1, 5, previous);

                ChunkDecoratorData tree;
                tree.name = "deciduous_tree" + previous;
                tree.properties = new Dictionary<string, object>();
                tree.properties.Add("position", new Vector3(
                    Mathf.Lerp(0, Chunk.CHUNK_SIZE, (float)i / trees),
                    0,
                    4));
                chunk.Add(tree);
            }

            //Generate terrain
            for (int i = 0; i < Chunk.CHUNK_SIZE / 4; i++)
            {
                ChunkDecoratorData terrain;
                terrain.name = "terrain1_center";
                terrain.properties = new Dictionary<string, object>();
                terrain.properties.Add("position", new Vector3(i * 4, 0, 5));
                chunk.Add(terrain);
            }

            return chunk;
        }
    }
}