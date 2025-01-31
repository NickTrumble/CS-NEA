using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace NEA_Procedural_World_Generator
{
    //class used to export the noisemap to an obj file
    public class OBJExport
    {
        Dictionary<(int x, int y), Chunk> inTerrain;
        World world;
        public float[,] terrain { get; set; }
        public int row { get; set; }
        public int col { get; set; }
        public float MeshScale { get; set; }
        public float[,] ATerrain;
        //constructor
        public OBJExport(World inworld, float Meshscale)
        {
            world = inworld;
            inTerrain = world.WorldChunks;
            MeshScale = Meshscale;

        }

        public async Task SaveAll(string path)//save whole woruld
        {
            row = col = world.Size;
            ATerrain = world.DictionaryToArray(inTerrain, row, col, 0, 0);
            await ExportOBJ(path);
        }

        public async void SaveRegion(int xlow, int ylow, int xhigh, int yhigh, string path)//save region selected
        {
            inTerrain = (Dictionary<(int x, int y), Chunk>)inTerrain.Where(c => c.Key.x >= xlow && c.Key.x <= xhigh
                                                                            && c.Key.y >= ylow && c.Key.y <= yhigh);
            row = xhigh - xlow + 1;
            col = yhigh - ylow + 1;
            ATerrain = world.DictionaryToArray(inTerrain, row, col, xlow, ylow);
            await ExportOBJ(path);
        }



        //generates the vertices used in the obj file 
        public List<(float, float, float)> Generate_vertices()
        {
            List<(float, float, float)> vertices = new List<(float, float, float)>();

            int worldrow = World.chunkSize * row;
            int worldcol = World.chunkSize * col;
            for (int i = 0; i < worldrow; i++)
            {
                for (int j = 0; j < worldcol; j++)
                {
                    float x = i * MeshScale;
                    float y = ATerrain[i, j];
                    float z = j * MeshScale;
                    vertices.Add((x, y, z));
                }
            }
            return vertices;
        }

        //generates a list of indexed vertices connected to make faces
        public List<(int, int, int)> Gen_faces()
        {
            List<(int, int, int)> faces = new List<(int, int, int)>();
            int worldrow = World.chunkSize * row;
            int worldcol = World.chunkSize * col;

            for (int i = 0; i < worldrow - 2; i++)
            {
                for (int j = 0; j < worldcol - 2; j++)
                {
                    int tl = i * worldrow + j + 1;
                    int tr = tl + 1;
                    int bl = tl + worldrow;
                    int br = bl + 1;
                    faces.Add((tl, tr, br));
                    faces.Add((tl, br, bl));
                }
            }
            return faces;
        }

        //combines all functions to one to export the map
        public async Task ExportOBJ(string path)
        {
            int multiplyer = 2;
            StreamWriter f = new StreamWriter(path + "\\TerrainMesh.obj");
            List<(float, float, float)> vertices = Generate_vertices();
            List<(int, int, int)> faces = Gen_faces();
            //MessageBox.Show($"faces:{faces.Count()} vertices:{vertices.Count()}");

            foreach (var v in vertices)
            {
                await f.WriteLineAsync($"v {v.Item1} {v.Item2 * multiplyer} {v.Item3}");
            }

            foreach (var face in faces)
            {
                await f.WriteLineAsync($"f {face.Item1} {face.Item2} {face.Item3}");
            }
            _ = MessageBox.Show("saved");

        }
    }


}