using Assets.Scripts.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChunkGenerators
{
    public class DecidousChunkGenerator
    {
        private static System.Random globalRandom = new System.Random();

        public DecidousChunkGenerator(
            int trees = 6,
            int saplings = 4,
            int bushes = 4,
            int shrubs = 10,
            int fenrs = 0)
        {
            this.trees = trees;
            this.saplings = saplings;
            this.bushes = bushes;
            this.shrubs = shrubs;
        }

        public int trees = 6;
        public int saplings = 4;
        public int bushes = 4;
        public int shrubs = 10;
        public int ferns = 10;

        public static DecidousChunkGenerator Forest = new DecidousChunkGenerator(6, 2, 3, 2, 8);
        public static DecidousChunkGenerator LightForest = new DecidousChunkGenerator(3, 2, 1, 2, 4);
        public static DecidousChunkGenerator Grassland = new DecidousChunkGenerator(1, 1, 1, 3, 0);

        public List<ChunkDecoratorData> GenerateForest(int seed = int.MinValue)
        {
            if (seed == int.MinValue)
                seed = globalRandom.Next();

            List<ChunkDecoratorData> chunk = new List<ChunkDecoratorData>();

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

            float offset = (float)Chunk.CHUNK_SIZE / 2 / saplings;
            int rand = (int)(offset / 2.5f);
            for (int i = 0; i < saplings; i++)
            {
                previous = rng.NextExcluding(1, 5, previous);

                ChunkDecoratorData sapling;
                sapling.name = "deciduous_sapling" + previous;
                sapling.properties = new Dictionary<string, object>();
                sapling.properties.Add("position",
                    new Vector3(
                        Mathf.Lerp(0, Chunk.CHUNK_SIZE, (float)i / saplings)
                        + rng.Next(-rand, rand) * rand + offset,
                        0,
                        1));
                chunk.Add(sapling);
            }

            //generate shrubs
            previous = 0;

            offset = (float)Chunk.CHUNK_SIZE / 2 / shrubs;
            rand = (int)(offset / 2.5f);
            for (int i = 0; i < shrubs; i++)
            {
                previous = rng.NextExcluding(1, 5, previous);

                ChunkDecoratorData bush;
                bush.name = "deciduous_shrub" + previous;
                bush.properties = new Dictionary<string, object>();
                bush.properties.Add("position",
                    new Vector3(
                    Mathf.Lerp(0, Chunk.CHUNK_SIZE, (float)i / shrubs)
                     + rng.Next(-rand, rand) * rand + offset,
                    0,
                    2));
                chunk.Add(bush);
            }

            //generate bush
            previous = 0;

            offset = (float)Chunk.CHUNK_SIZE / 2 / bushes;
            rand = (int)(offset / 2.5f);
            for (int i = 0; i < bushes; i++)
            {
                previous = rng.NextExcluding(1, 5, previous);

                ChunkDecoratorData bush;
                bush.name = "deciduous_bush" + previous;
                bush.properties = new Dictionary<string, object>();
                bush.properties.Add("position", new Vector3(
                    Mathf.Lerp(0, Chunk.CHUNK_SIZE, (float)i / bushes)
                      + rng.Next(-rand, rand) * rand + offset,
                    0,
                    3));
                chunk.Add(bush);
            }

            //generate trees
            previous = 0;

            offset = (float)Chunk.CHUNK_SIZE / 2 / trees;
            rand = (int)(offset / 2.5f);
            for (int i = 0; i < trees; i++)
            {
                previous = rng.NextExcluding(1, 5, previous);

                ChunkDecoratorData tree;
                tree.name = "deciduous_tree" + previous;
                tree.properties = new Dictionary<string, object>();
                tree.properties.Add("position", new Vector3(
                    Mathf.Lerp(0, Chunk.CHUNK_SIZE, (float)i / trees)
                       + rng.Next(-rand, rand) + offset,
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