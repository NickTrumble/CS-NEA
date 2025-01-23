using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace NEA_Procedural_World_Generator
{
    //class to handle all aspects of the world in short term and longterm
    public class World
    {
        //public variables
        public Dictionary<(int x, int y), Chunk> WorldChunks;
        public int Size, Octaves;
        public BaseNoise Noise;
        public static int chunkSize = 32;
        public float Persistance, Scale;
        public Stack<List<Chunk>> UndoStack, RedoStack;
        public List<Chunk> temp;


        

        //Color.FromArgb(59, 107, 161), Color.FromArgb(232, 235, 56),
        //    Color.FromArgb(39, 99, 21), Color.FromArgb(88, 57, 39), 
        //    Color.FromArgb(98, 100, 102)

        //private variables

        public World(int size, int octaves, float pers, float scale)
        {
            Size = size;
            WorldChunks = new Dictionary<(int x, int y), Chunk>();
            UndoStack = new Stack<List<Chunk>>();
            RedoStack = new Stack<List<Chunk>>();
            temp = new List<Chunk>();
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
        public Block.BlockState BlockStateTransformer(float val)
        {
            if (val < 0.3)
            {
                return Block.BlockState.Water;
            }
            else if (val < 0.6)
            {
                return Block.BlockState.Sand;
            }
            else if (val < 0.75)
            {
                return Block.BlockState.Grass;
            }
            else if (val < 0.9)
            {
                return Block.BlockState.Dirt;
            }
            else
            {
                return Block.BlockState.Stone;
            }

        }

        public static Color BlockColourTransformer(Block block)
        {
            //int index = (int)block.BlockType;
            //Color c = BlockRGB[index];
            //return c;
            return TerrainCmap.Interpolate_value(block.Z);
        }

        public static float min = 1;
        public static float max = -1;

        public void WorldGeneration()
        {
            List<Chunk> chunks = new List<Chunk>();

            for (int i = 0; i < Form1.world.Size; i++)
            {
                for (int j = 0; j < Form1.world.Size; j++)
                {
                    Chunk c = Chunk.ChunkGeneration(new Chunk(i, j));
                    WorldChunks[(i, j)] = c;
                    chunks.Add(c);
                }

            }
            temp = CloneWorld(WorldChunks.Values.ToList());
            Form1.UI.TerrainBox.Invalidate();
        }

        public void EditWorld(int x, int y, int radius, float intensity)//split up into other functions
        {
            float offsetx = (Form1.xoff * chunkSize);
            float offsety = (Form1.yoff * chunkSize);

            int clickedx = (int)(x + offsetx);

            int xmin = (int)Math.Max(0, x - radius + offsetx);
            int ymin = (int)Math.Max(0, y - radius + offsety);

            int xmax = (int)Math.Min(chunkSize * Size, x + radius + offsetx);
            int ymax = (int)Math.Min(chunkSize * Size, y + radius + offsety);

            int radius2 = radius * radius;
            Parallel.For(xmin, xmax, i =>
            {
                for (int j = ymin; j < ymax; j++)
                {
                    float distance = (float)(Math.Pow(i - x - offsetx, 2) + Math.Pow(j - y - offsety, 2));
                    if (distance < radius2)
                    {
                        float incVal = intensity * (1 - (distance / (radius2)));
                        int X = Math.Min((int)(i / chunkSize), Size - 1);
                        int Y = Math.Min((int)(j / chunkSize), Size - 1);

                        //get chunk reference
                        Chunk chunk = Form1.world.WorldChunks[(X, Y)];
                        //get block reference
                        int blockx = i % chunkSize;
                        int blocky = j % chunkSize;
                        Block block = chunk.ChunkBlock[(blockx, blocky)];
                        block.Z += incVal;

                        //edit block state and colour
                        block.BlockType = BlockStateTransformer(block.Z);
                        Color c = BlockColourTransformer(chunk.ChunkBlock[(blockx, blocky)]);

                        //set pixel colour//update to faster version if possible
                        lock (chunk.Bmp)
                        {
                            chunk.Bmp.SetPixel(blockx, blocky, c);
                        }


                    }
                }

            });
            
        }

        public static List<Chunk> CloneWorld(List<Chunk> world)
        {
            List<Chunk> Cloned = new List<Chunk>();
            foreach(Chunk c in world)
            {
                Cloned.Add((Chunk)((ICloneable)c).Clone());
            }
            return Cloned;
        }

        public float[,] DictionaryToArray(Dictionary<(int, int), Chunk> inD, int row, int col, int xoffset, int yoffset)
        {
            int width = row * chunkSize;
            int height = col * chunkSize;
            

            float[,] newA = new float[width, height];
            for (int i = 0; i < row ; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    for (int k = 0; k < chunkSize; k++)
                    {
                        for (int l = 0; l < chunkSize; l++)
                        {
                            newA[i * chunkSize + k, j * chunkSize + l] = WorldChunks[(i + xoffset, j + yoffset)].ChunkBlock[(k, l)].Z;
                        }
                    }
                }
            }
            return newA;
        }
    }

    //class to handle chunks of the world to generate easier
    public class Chunk : ICloneable
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

                    Block.BlockState state = Form1.world.BlockStateTransformer(noisevalue);
                    chunk.ChunkBlock[(i, j)].BlockType = state;

                    chunk.Bmp.SetPixel(i, j, World.BlockColourTransformer(chunk.ChunkBlock[(i, j)]));

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

        object ICloneable.Clone()
        {
            Chunk chunk = new Chunk(X, Y);
            chunk.Bmp = new Bitmap(this.Bmp);
            foreach(var block in ChunkBlock)
            {
                chunk.ChunkBlock[block.Key] = (Block)block.Value.Clone();
            }
            return chunk;
        }

    }

    //class to handle all block properties
    public class Block : ICloneable
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

        public object Clone()
        {
            return new Block(X, Y)
            {
                Z = this.Z,
                BlockType = this.BlockType,
                Biome = this.Biome
            };
        }

        
    }


}
