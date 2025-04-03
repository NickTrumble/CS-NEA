using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace NEA_Procedural_World_Generator
{
    public class DrawMesh
    {
        #region setup
        //public variables
        public World inWorld { get; set; }//internal world used to map the dictionary to an array
        public float[,] heightmap { get; set; }//array versiuon of the terrain

        //initiaise public variables
        public static Color[] Colours = new Color[50];
        public int xoffset, yoffset, xLow, xHigh, yLow, yHigh, row, col, tilewidth, tileheight, Scale, half;
        public DrawMesh(World world, int scale, int xlow, int xhigh, int ylow, int yhigh)
        {
            //assigning variables as internal variants
            Scale = scale;
            inWorld = world;

            //figure out the smaller of the two inputs, as user can drag up and left
            //which would give the impression that xhigh be smaller than xlow
            //if xlow is smaller than xhigh, make xLow = xlow
            xLow = (xlow < xhigh) ? xlow : xhigh;
            //check if xLow == xlow, if true then xHigh = xhigh
            xHigh = (xLow == xlow) ? xhigh : xlow;
            //repeat for y coordinates
            yLow = (ylow < yhigh) ? ylow : yhigh;
            yHigh = (yLow == ylow) ? yhigh : ylow;
            //calculate the width and height of the cropped terrain in chunks
            //the +1 accounts for the inclusivity
            row = xHigh - xLow + 1;
            col = yHigh - yLow + 1;

            //convert the Dictionary holding the terrain to a cropped array
            heightmap = world.DictionaryToArray(row, col, xlow, ylow);
            //calculate the distance between each vertice when displayed
            tilewidth = Form1.UI.TerrainBox.Width / heightmap.GetLength(0);
            tileheight = Form1.UI.TerrainBox.Height / heightmap.GetLength(1);
            //centre of terrain used for point calculations
            half = heightmap.GetLength(0) / 2;
            
            //calculate offsets to centre the terrain in the picturebox
            yoffset = 2 * Form1.UI.TerrainBox.Height / 3;
            xoffset = 2 * Form1.UI.TerrainBox.Width / 5;

            //pre-calculate 50 colours that the terrain can round to to make
            //could be adjusted to provide a less clean look
            for (int i = 0; i < 50; i++)
            {
                Colours[i] = TerrainCmap.Interpolate_value(i / 50f);
            }
        }
        #endregion

        //takes the rotation matrix and vertex point in 3d and converts into 2d screen coordinates
        public Point PointCalc(Vector3 V, Matrix3x2 rMatrix)
        {
            /*          [cos(x)  0  -sin(x)
             * R(x) =      0     1     0
             *          sin(x)   0   cos(x)]
             */
            //applies rotation matrix to vertex
            Vector2 transformed = Vector2.Transform(new Vector2(V.X - half, V.Z - half), rMatrix);
            //applies the second transformation to the points to centre them and 
            //enlarge them to the correct size
            int X = (int)(transformed.X * tilewidth * 0.5f + xoffset);
            int Y = (int)(transformed.Y * tileheight * 0.25f + yoffset - V.Y);

            return new Point(X, Y);
        }

        //inputs the group of 4 corner vertices into the PointCalc function
        //to collect an array of screen coordinate points to return,
        //takes input as vectors from VectorCalc
        public Point[] cornerCalc(Vector3[] Vectors, Matrix3x2 rMatrix)
        {
            Point[] corners = new Point[4];
            corners[0] = PointCalc(Vectors[0], rMatrix);
            corners[1] = PointCalc(Vectors[1], rMatrix);
            corners[2] = PointCalc(Vectors[2], rMatrix);
            corners[3] = PointCalc(Vectors[3], rMatrix);

            return corners;
        }

        //returns an array of 3d vectors to be used in the CornerCalc
        public Vector3[] VectorCalc(int i, int j)
        {
            //the vectors created create a square with the original being bottom left
            Vector3[] Vectors = new Vector3[4];
            Vectors[0] = new Vector3(i, heightmap[i, j] * Scale, j);//bottom left
            Vectors[1] = new Vector3(i + 1, heightmap[i + 1, j] * Scale, j);//bottom right
            Vectors[2] = new Vector3(i, heightmap[i, j + 1] * Scale, j + 1);//top left
            Vectors[3] = new Vector3(i + 1, heightmap[i + 1, j + 1] * Scale, j + 1);//top right

            return Vectors;
        }

        //the main function that puts it all together, and draws on the buffer bitmap
        public void Draw(Graphics g, float angle)
        {
            //create the rotation matrix for the angle in radians
            Matrix3x2 Rmatrix = Matrix3x2.CreateRotation((float)(angle * Math.PI / 180));
            //size of the cropped terrain in blocks
            int sizex = heightmap.GetLength(0);
            int sizey = heightmap.GetLength(1);
            //arrays to hold the data about the points
            Point[] corners;
            Vector3[] Vectors;
            //initialise the brush to prevent reinitialising every loop and wasting resources
            SolidBrush b = new SolidBrush(Color.White);
            
            //loop over all blocks in the terrain - 2 to account 
            //for 0 based arrays, and the +1 in vectorcalc
            for (int i = 0; i != sizex - 2; i += 1)
            {
                for (int j = 0; j != sizey - 2; j += 1)
                {
                    //populate arrays from earlier
                    Vectors = VectorCalc(i, j);
                    corners = cornerCalc(Vectors, Rmatrix);
                    
                    //calculate the average elevation of each simplex
                    float avg1 = (heightmap[i, j] + heightmap[i + 1, j] + heightmap[i, j + 1]) / 3;
                    float avg2 = (heightmap[i + 1, j + 1] + heightmap[i + 1, j] + heightmap[i, j + 1]) / 3;
                    //update colours on brush, rounding to one of the 50 colours from init.
                    b.Color = Colours[(int)(49 * Math.Min(1, Math.Max(0, avg1)))];
                    //fill the triangle made up of 3 of the points
                    g.FillPolygon(b, new Point[3] { corners[2], corners[0], corners[1] });

                    //update colours on brush, rounding to one of the 50 colours from init.
                    b.Color = Colours[(int)(49 * Math.Min(1, Math.Max(0, avg2)))];
                    //fill the triangle made up of 3 of the points
                    g.FillPolygon(b, new Point[3] { corners[2], corners[1], corners[3] });
                }
            }
        }
    }

}
