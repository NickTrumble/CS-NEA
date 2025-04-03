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
        public Dictionary<(int x, int y), Chunk> WorldChunks;//contains a list of world chunks and their coordinates
        public int Size, Octaves;
        public BaseNoise Noise;
        public static int chunkSize = 32;
        public float Persistance, Scale;
        public Stack<List<Chunk>> UndoStack, RedoStack;//holds a non-reference copy of the world chunks before/after editing
        public List<Chunk> temp;//holds last non reference world chunks for undo stack

        public World(int size, int octaves, float pers, float scale)
        {
            //assigning basic variables
            Persistance = pers;
            Octaves = octaves;
            Scale = scale;
            Size = size;
            //initialising generic data types
            WorldChunks = new Dictionary<(int x, int y), Chunk>();
            UndoStack = new Stack<List<Chunk>>();
            RedoStack = new Stack<List<Chunk>>();
            temp = new List<Chunk>();

            //initialises noise based on the noise method selected in the options
            if (InterfaceHandler.NoiseMethodd == InterfaceHandler.NoiseState.Perlin)
            {
                Noise = new PerlinNoise(size * chunkSize, octaves, pers);
            }
            else
            {
                Noise = new SimplexNoise(size * chunkSize, octaves, pers);
            }

            
        }

        public static Color BlockColourTransformer(Block block)
        {
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

        public void EditWorld(int x, int y, int radius, float intensity)//split up into other functions//add for zoom
        {
            x /= InterfaceHandler.zoom; //adjust for zoom
            y /= InterfaceHandler.zoom;

            float offsetx = Form1.xoff * chunkSize;//offsets already account for zoom
            float offsety = Form1.yoff * chunkSize;

            int clickedx = (int)(x + offsetx);//find the exact coordinate clicked in zoom

            int xmin = (int)Math.Max(0, x - radius + offsetx);//find the square encapsulating the circle of influence
            int ymin = (int)Math.Max(0, y - radius + offsety);//topleft corner

            int xmax = (int)Math.Min(chunkSize * Size, x + radius + offsetx);//bottom right corner
            int ymax = (int)Math.Min(chunkSize * Size, y + radius + offsety);

            int radius2 = radius * radius;
            //loop over all blocks in the square of influence
            Parallel.For(xmin, xmax, i =>
            {
                for (int j = ymin; j < ymax; j++)
                {
                    //compare if block is inside the clicked blocks circle of influence
                    float distance = (float)(Math.Pow(i - x - offsetx, 2) + Math.Pow(j - y - offsety, 2));
                    if (distance < radius2)
                    {
                        //inverse square law to make a more natural fall off
                        float incVal = intensity * (1 - (distance / radius2));
                        //finds chunk of block
                        int X = Math.Min(i / chunkSize, Size - 1);
                        int Y = Math.Min(j / chunkSize, Size - 1);

                        //get chunk reference
                        Chunk chunk = Form1.world.WorldChunks[(X, Y)];
                        //get block position in chunk
                        int blockx = i % chunkSize;
                        int blocky = j % chunkSize;

                        //find block reference
                        Block block = chunk.ChunkBlock[(blockx, blocky)];
                        //increase elevation by inverse square law
                        block.Z += incVal;
                        //get new colour for chunk bitmap
                        Color c = BlockColourTransformer(block);

                        //set pixel colour//update to faster version if possible
                        //lock bitmap due for threadsafety during parallel processing
                        lock (chunk.Bmp)
                        {
                            chunk.Bmp.SetPixel(blockx, blocky, c);
                        }


                    }
                }

            });

        }

        //Clones the world without reference 
        public static List<Chunk> CloneWorld(List<Chunk> world)
        {
            //creates a new list of chunkns to replace WorldChunks
            List<Chunk> Cloned = new List<Chunk>();
            //loop over all the chunks and call the ICloneable Method
            foreach (Chunk c in world)
            {
                //clone each chunk then add to new list
                Cloned.Add(c.Clone());
            }
            return Cloned;
        }

        //converts dictionaries like WorldChunks to an array
        public float[,] DictionaryToArray(int row, int col, int xoffset, int yoffset)
        {
            //widht and height of terrain in blocks, chunks * chunksize
            int width = row * chunkSize;
            int height = col * chunkSize;
            
            //array holding the new data
            float[,] newA = new float[width, height];
            //loop over blocks in a heirarchy: chunk x and y -> block x and y
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    for (int k = 0; k < chunkSize; k++)
                    {
                        for (int l = 0; l < chunkSize; l++)
                        {
                            //populates new array with old data
                            newA[i * chunkSize + k, j * chunkSize + l] = WorldChunks[(i + xoffset, j + yoffset)].ChunkBlock[(k, l)].Z;
                        }
                    }
                }
            }
            return newA;
        }
    }

    //class to handle chunks of the world to generate easier
    public class Chunk
    {
        //public variables
        public Dictionary<(int x, int y), Block> ChunkBlock;//holds all block data in the chunk
        //coordinates of the chunk
        public int X;
        public int Y;
        //bitmap of the chunk
        public Bitmap Bmp;

        public Chunk(int x, int y)
        {
            //initialise and assign basic internal variables
            ChunkBlock = new Dictionary<(int x, int y), Block>();
            Bmp = new Bitmap(World.chunkSize, World.chunkSize);
            X = x;
            Y = y;
        }

        //generate nosie values for all blocks in inputted chunk
        public static Chunk ChunkGeneration(Chunk chunk)
        {
            //frequency to pass into noise methods
            float freq = Form1.world.Scale;
            //iterate over all blocks to put into noise functions
            for (int i = 0; i < World.chunkSize; i++)
            {
                for (int j = 0; j < World.chunkSize; j++)
                {
                    //calculates the noise value for specific block,
                    float noisevalue = Math.Min(1, 0.5f + Form1.world.Noise.Noise_method((chunk.X * World.chunkSize + i) * freq, (chunk.Y * World.chunkSize + j) * freq));
                    //declare new block at i, j, where elevation = noise value
                    chunk.ChunkBlock[(i, j)] = new Block(i, j);
                    chunk.ChunkBlock[(i, j)].Z = noisevalue;
                    //udpate the bitmap with the new value
                    chunk.Bmp.SetPixel(i, j, World.BlockColourTransformer(chunk.ChunkBlock[(i, j)]));

                    if (noisevalue < World.min)
                    {
                        World.min = noisevalue;
                    }
                    if (noisevalue > World.max)
                    {
                        World.max = noisevalue;
                    }//used for debugging//remove
                }

            }
            return chunk;

        }
        //copy each blockk in tthe chunk and return
        public Chunk Clone()
        {
            //generate the new chunk
            Chunk chunk = new Chunk(X, Y);
            chunk.Bmp = new Bitmap(this.Bmp);//clone the bitmap
            foreach (var block in ChunkBlock)
            {
                //copy values of elevation for the blocks over
                chunk.ChunkBlock[block.Key] = block.Value.Clone();
            }
            return chunk;
        }
    }

    //class to handle all block properties
    public class Block
    {
        //public variables
        public int X;
        public int Y;
        public float Z;

        public Block(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Block Clone()
        {
            return new Block(X, Y)
            {
                Z = this.Z,
            };
        }
    }


}