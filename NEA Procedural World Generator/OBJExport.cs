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

        //public variables
        public Dictionary<(int x, int y), Chunk> inTerrain;//Terrain as a dictionary inputted from the Main Form
        public World World;//World inputted from the Main Form
        public static string FileName = "TerrainMesh";//Stores the file name, defaukt is "TerrainMesh"
        public int row;//how many rows long the terrain is after being cropped
        public int col;//how many columns long the terrain is after being cropped
        public float MeshScale;//ths scale the elevation of the terrain will be when generating the mesh to be saved
        public float[,] ATerrain;//Array representing the input terrain after cropping, easier to read thatn dictionary

        //constructor
        public OBJExport(World inworld, float Meshscale)
        {
            //Store inputted variables as their internal corresponding variables
            World = inworld;
            inTerrain = World.WorldChunks;
            MeshScale = Meshscale;
        }

        //async to call the await async function
        public async Task SaveAll(string path)//save whole woruld
        {
            //if whole world is saved, it will be square, therefore the width, and height
            //are equal to the size of the world in chunks
            row = col = World.Size;

            //changes the terrain from a dicctionary ot array so its eassier to read
            ATerrain = World.DictionaryToArray(row, col, 0, 0);
            //await allows the program to stay running while the task is executed
            //begins to write to a new file at path 
            await Task.Run(() => ExportOBJ(path));
        }

        //async to call the await async function
        public async Task SaveRegion(int xlow, int ylow, int xhigh, int yhigh, string path)//save region selected
        {
            //crops the terrain by using LINQ to get rid of the values outside the boudaries
            //then converting back into a dicitonary otherwise will be in a weird LINQ format
            inTerrain = inTerrain.Where(c => c.Key.x >= xlow && c.Key.x <= xhigh && c.Key.y >= ylow && c.Key.y <= yhigh)
                .ToDictionary(c => c.Key, c => c.Value);
            //region width and height, +1 for the inclusive part, e.g. chunk 0 to chunk 0 is 1 wide
            row = xhigh - xlow + 1;
            col = yhigh - ylow + 1;
            //convert the whole world into cropped terrain with new boundaries
            ATerrain = World.DictionaryToArray(row, col, xlow, ylow);
            //await allows the program to stay running while the task is executed
            //begins to write to a new file at path 
            await Task.Run(() => ExportOBJ(path));
        }



        //generates the vertices used in the obj file 
        public List<(float, float, float)> Generate_vertices()
        {
            //list holding the i, j, k values of every vertex
            List<(float, float, float)> vertices = new List<(float, float, float)>();
            //world size in blocks, e.g. round coordinates that the vertices are stored in
            int worldrow = World.chunkSize * row;
            int worldcol = World.chunkSize * col;
            //loops over every coordinate in cropped terrain to create the mesh to save
            for (int i = 0; i < worldrow; i++)
            {
                for (int j = 0; j < worldcol; j++)
                {
                    //create a vertice from the x, y, and z
                    float x = i / MeshScale;//divided by meshscale to scale down, similar to y
                    float y = ATerrain[i, j];
                    float z = j / MeshScale;
                    //add vertiecs to list
                    vertices.Add((x, y, z));
                }
            }
            return vertices;
        }

        //generates a list of indexed vertices connected to make faces
        public List<(int, int, int)> Gen_faces()
        {
            //make a new list to hold groups of 3 vertex indicies used in faces
            List<(int, int, int)> faces = new List<(int, int, int)>();
            //world size in blocks, e.g. round coordinates that the vertices are stored in
            int worldrow = World.chunkSize * row;
            int worldcol = World.chunkSize * col;
            //loops over every coordinate in cropped terrain to create the mesh to save
            for (int i = 0; i < worldrow - 2; i++)
            {
                for (int j = 0; j < worldcol - 2; j++)
                {
                    //i*worldcol represents the amount of columns the loop has traversed
                    //j+1 represents the amount of blocks on the specific column the loop is on
                    int tl = i * worldcol + j + 1;
                    int tr = tl + 1;
                    //adds the width of the world to link the next column to this one
                    int bl = tl + worldcol;
                    int br = bl + 1;
                    //create 2 meshes out of 4 vertices like its a kite cut in half
                    faces.Add((tl, tr, br));
                    faces.Add((tl, br, bl));
                }
            }
            return faces;
        }

        //combines all functions to one to export the map
        public async Task ExportOBJ(string path)
        {
            //gets the list of vertices and faces
            List<(float, float, float)> vertices = Generate_vertices();
            List<(int, int, int)> faces = Gen_faces();
            //handles different formats for saving based on file type selected
            //as each file type requires different formats to be read
            switch (InterfaceHandler.FileType)
            {
                //obj file format selected
                case InterfaceHandler.SaveFileType.OBJ:                    
                    //create streamwriter to write the save file for the given file type, name and directory
                    using (StreamWriter f = new StreamWriter(path + $"\\{FileName}.obj"))
                    {
                        //async write all the vertices so that the program stays running
                        foreach (var v in vertices)
                        {
                            //v in front of the line to declare as a vertice
                            await f.WriteLineAsync($"v {v.Item1} {v.Item2} {v.Item3}");
                        }
                        //async write all the faces so that the program stays running
                        foreach (var face in faces)
                        {
                            //f in front of the line to declare as a face
                            await f.WriteLineAsync($"f {face.Item1} {face.Item2} {face.Item3}");
                        }
                    }
                    break;
                //ply format chosen by user
                default:
                    //create streamwriter to write the save file for the given file type, name and directory
                    using (StreamWriter f = new StreamWriter(path + $"\\{FileName}.ply"))
                    {
                        //formatting for .PLY files
                        f.WriteLine("ply");//file type
                        f.WriteLine("format ascii 1.0");//what charset used
                        //make new element and how many there will be
                        f.WriteLine($"element vertex {vertices.Count}");
                        //declare properties of element and types
                        f.WriteLine("property float x");
                        f.WriteLine("property float y");
                        f.WriteLine("property float z");
                        //make new element and how many there will be
                        f.WriteLine($"element face {faces.Count}");
                        //a list of (small integers) holding integers called vertex indices
                        f.WriteLine("property list uchar int vertex_indices");
                        f.WriteLine("end_header");//declears the actual data is after this

                        //async write all the vertices so that the program stays running
                        foreach (var v in vertices)
                        {
                            await f.WriteLineAsync($"{v.Item1} {v.Item2} {v.Item3}");
                        }

                        //async write all the faces so that the program stays running
                        foreach (var face in faces)
                        {
                            //3 at the start of the line is the uchar part, declaring how many properties there will be
                            await f.WriteLineAsync($"3 {face.Item1 - 1} {face.Item2 - 1} {face.Item3 - 1}");
                        }
                    }

                    break;
            }           
            MessageBox.Show($"Saved as .{InterfaceHandler.FileType}!");

        }
    }


}