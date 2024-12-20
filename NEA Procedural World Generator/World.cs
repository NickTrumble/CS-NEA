using System;
using System.Collections.Generic;
using System.Drawing;

namespace NEA_Procedural_World_Generator
{
    //class to handle all aspects of the world in short term and longterm
    public class World
    {
        //public variables
        public Dictionary<(int x, int y), Chunk> WorldChunks;
        public Dictionary<(int x, int y), Chunk> InhabitedChunks;
        public int Size, Octaves;
        public BaseNoise Noise;
        public static int chunkSize = 32;
        public float Persistance, Scale;


        public static Color[] BlockRGB = new Color[5]
        {
            Color.FromArgb(59, 107, 161), Color.FromArgb(232, 235, 56),
            Color.FromArgb(39, 99, 21), Color.FromArgb(88, 57, 39), 
            Color.FromArgb(98, 100, 102)
        };

        //private variables

        public World(int size, int octaves, float pers, float scale)
        {
            Size = size;
            WorldChunks = new Dictionary<(int x, int y), Chunk>();
            InhabitedChunks = new Dictionary<(int x, int y), Chunk>();
            if (InterfaceHandler.NoiseMethodd == InterfaceHandler.NoiseState.Perlin)
            {
                Noise = new PerlinNoise(size * chunkSize, octaves, pers);
            } else
            {
                Noise = new SimplexNoise(size * chunkSize, octaves, pers);
            }
            
            Persistance = pers;
            Octaves = octaves;
            Scale = scale;
        }

        //gives a block a block state through biome and height
        public Block.BlockState BlockStateTransformer(Block block, float val)
        {
            if (val < 0.3)
            {
                return Block.BlockState.Water;
            }
            else if (val < 0.4)
            {
                return Block.BlockState.Sand;
            }
            else if (val < 0.8)
            {
                return Block.BlockState.Grass;
            }
            else
            {
                return Block.BlockState.Stone;
            }

        }

        public static Color BlockColourTransformer(Block block, float val)
        {
            int index = (int)(block.BlockType);
            Color c = BlockRGB[index];
            return c;
            //return Color.FromArgb((int)(val * 255), (int)(val * 255), (int)(val * 255));
        }

        public static float min = 1;
        public static float max = -1;

        public void WorldGeneration()
        {
            for (int i = 0; i < Form1.world.Size; i++)
            {
                for (int j = 0; j < Form1.world.Size; j++)
                {
                    Form1.world.WorldChunks[(i, j)] = Chunk.ChunkGeneration(new Chunk(i, j));

                }

            }
            Form1.UI.TerrainBox.Invalidate();
        }

    }

    //class to handle chunks of the world to generate easier
    public class Chunk
    {
        //public variables
        public Dictionary<(int x, int y), Block> ChunkBlock;
        public int X;
        public int Y;
        public Bitmap Bmp;

        //private variables
        private Random rnd = new Random();

        public Chunk(int x, int y)
        {
            ChunkBlock = new Dictionary<(int x, int y), Block>();
            X = x;
            Y = y;
            Bmp = new Bitmap(World.chunkSize, World.chunkSize);
        }

        public static Chunk ChunkGeneration(Chunk chunk) 
        {
            float freq = Form1.world.Scale;
            for (int i = 0; i < World.chunkSize; i++)
            {
                for (int j = 0; j < World.chunkSize; j++)
                {

                    float noisevalue = Math.Min(1, 0.5f + Form1.world.Noise.Noise_method((chunk.X * World.chunkSize + i) * freq, (chunk.Y * World.chunkSize + j) * freq));
                    chunk.ChunkBlock[(i, j)] = new Block(i, j);
                    chunk.ChunkBlock[(i, j)].Z = noisevalue;

                    Block.BlockState state = Form1.world.BlockStateTransformer(chunk.ChunkBlock[(i, j)], noisevalue);
                    chunk.ChunkBlock[(i, j)].BlockType = state;

                    chunk.Bmp.SetPixel(i, j, World.BlockColourTransformer(chunk.ChunkBlock[(i, j)], noisevalue));

                    if (noisevalue < World.min)
                    {
                        World.min = noisevalue;
                    }
                    if (noisevalue > World.max)
                    {
                        World.max = noisevalue;
                    }//used for debugging
                }

            }
            return chunk;

        }
    }

    //class to handle all block properties
    public class Block
    {
        //public variables
        //enums
        public enum BlockState { Water, Sand, Grass, Dirt, Stone }
        public enum BiomeState { Desert, Plains, Mountains }//add biomes

        public BlockState BlockType;
        public BiomeState Biome;

        public int X;
        public int Y;
        public float Z;
        //private variables

        public Block(int x, int y)
        {
            BlockType = BlockState.Water;
            Biome = BiomeState.Plains;
            X = x;
            Y = y;
        }
    }

}
