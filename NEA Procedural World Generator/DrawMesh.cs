using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace NEA_Procedural_World_Generator
{
    public class DrawMesh
    {
        #region setup
        public World inWorld { get; set; }
        public float[,] heightmap { get; set; }
        public int xoffset = 300;
        public int yoffset = 200;
        Color[] Colours = new Color[50];


        public int xLow, xHigh, yLow, yHigh, row, col, tilewidth, tileheight, Scale, half;
        public DrawMesh(World world, int scale, int xlow, int xhigh, int ylow, int yhigh)
        {
            xLow = (xlow < xhigh) ? xlow : xhigh;
            xHigh = (xLow == xlow) ? xhigh : xlow;
            yLow = (ylow < yhigh) ? ylow : yhigh;
            yHigh = (yLow == ylow) ? yhigh : ylow;

            row = xHigh - xLow + 1;
            col = yHigh - yLow + 1;
            inWorld = world;
            heightmap = inWorld.DictionaryToArray(inWorld.WorldChunks, row, col, xlow, ylow);

            tilewidth = Form1.UI.TerrainBox.Width / heightmap.GetLength(0);
            tileheight = Form1.UI.TerrainBox.Height / heightmap.GetLength(1);
            half = heightmap.GetLength(0) / 2;
            Scale = scale;
            yoffset = (int)(half * tileheight);
            xoffset = (int)(half * tilewidth);


            for (int i = 0; i < 50; i++)
            {
                Colours[i] = TerrainCmap.Interpolate_value(i / 50f);
            }
        }
        #endregion

        public Point PointCalc(Vector3 V, Matrix3x2 rMatrix)
        {
            Vector2 transformed = Vector2.Transform(new Vector2(V.X - half, V.Z - half), rMatrix);
            int X = (int)(transformed.X * tilewidth * 0.5f + xoffset);
            int Y = (int)(transformed.Y * tileheight * 0.25f + yoffset - V.Y);//yoffest = half * tilewidth * 0.25f

            return new Point(X, Y);
        }

        public Point[] cornerCalc(Vector3[] Vectors, Matrix3x2 rMatrix)
        {
            Point[] corners = new Point[4];
            corners[0] = PointCalc(Vectors[0], rMatrix);
            corners[1] = PointCalc(Vectors[1], rMatrix);
            corners[2] = PointCalc(Vectors[2], rMatrix);
            corners[3] = PointCalc(Vectors[3], rMatrix);

            return corners;
        }

        public Vector3[] VectorCalc(int i, int j)
        {
            Vector3[] Vectors = new Vector3[4];
            Vectors[0] = new Vector3(i, heightmap[i, j] * Scale, j);//bottom left
            Vectors[1] = new Vector3(i + 1, heightmap[i + 1, j] * Scale, j);//bottom right
            Vectors[2] = new Vector3(i, heightmap[i, j + 1] * Scale, j + 1);//top left
            Vectors[3] = new Vector3(i + 1, heightmap[i + 1, j + 1] * Scale, j + 1);//top right

            return Vectors;
        }

        public void Draw(Graphics g, float angle)
        {
            Matrix3x2 Rmatrix = Matrix3x2.CreateRotation((float)(angle * Math.PI / 180));
            int sizex = heightmap.GetLength(0);
            int sizey = heightmap.GetLength(1);
            Point[] corners;
            SolidBrush b = new SolidBrush(Color.White);
            Vector3[] Vectors;
            int endx = sizex - 2;
            int endy = sizey - 2;
            for (int i = 0; i != endx; i += 1)
            {
                for (int j = 0; j != endy; j += 1)
                {
                    Vectors = VectorCalc(i, j);
                    corners = cornerCalc(Vectors, Rmatrix);

                    float avg1 = (heightmap[i, j] + heightmap[i + 1, j] + heightmap[i, j + 1]) / 3;
                    float avg2 = (heightmap[i + 1, j + 1] + heightmap[i + 1, j] + heightmap[i, j + 1]) / 3;

                    b.Color = Colours[(int)(49 * Math.Min(1, Math.Max(0, avg1)))];
                    g.FillPolygon(b, new Point[3] { corners[2], corners[0], corners[1] });

                    b.Color = Colours[(int)(49 * Math.Min(1, Math.Max(0, avg2)))];
                    g.FillPolygon(b, new Point[3] { corners[2], corners[1], corners[3] });
                }
            }
        }

        public Color GetColour(int elevation)
        {
            int intensity = Math.Min(127, elevation * 13);
            return Color.FromArgb(2 * intensity, 2 * intensity, 128 + intensity);
        }
    }

}
