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

        public static Vector3 CamerDir = new Vector3(0, 0.5f, 0);
        Vector3 CameraPos;

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



            for (int i = 0; i < 50; i++)
            {
                Colours[i] = TerrainCmap.Interpolate_value(i / 50f);
            }
        }
        #endregion

        public Point PointCalc(Vector3 V, float cos, float sin)
        {
            float cx = V.X - half;
            float cy = V.Z - half;
            float hegiht = V.Y;

            int X = (int)((cos * cx - sin * cy) * tilewidth / 2f + xoffset);
            int Y = (int)((sin * cx + cos * cy) * tilewidth / 4f - hegiht + yoffset + half);

            return new Point(X, Y);
        }

        public Point[] cornerCalc(Vector3[] Vectors, float cos, float sin)
        {
            Point[] corners = new Point[4];
            corners[0] = PointCalc(Vectors[0], cos, sin);
            corners[1] = PointCalc(Vectors[1], cos, sin);
            corners[2] = PointCalc(Vectors[2], cos, sin);
            corners[3] = PointCalc(Vectors[3], cos, sin);

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

        public bool BackFace(Vector3[] points)
        {
            ////find top vertice
            ////top vertice - others = edge vectors
            ////get vector forwards/back and up/down in terms of camera directionm
            ////find gradient
            //// if gradient < 0 then draw
            ////discard
            //Vector3 center = (points[0] + points[1] + points[2]) / 3;
            //Vector3 camera = Vector3.Normalize(center - CamerDir);
            //Vector3 U = Vector3.Normalize(points[1] - points[0]);
            //Vector3 V = Vector3.Normalize(points[2] - points[0]);

            //Vector3 normal = Vector3.Normalize(Vector3.Cross(U, V));
            //float dot = Vector3.Dot(normal, camera);
            //if (dot < 0)
            //{
            //    return true;
            //}
            //return false;
            Vector3 center = (points[0] + points[1] + points[2]) / 3;
            
            Vector3 camera = Vector3.Normalize(center - CamerDir);

            Vector3 U = Vector3.Normalize(points[1] - points[0]);
            Vector3 V = Vector3.Normalize(points[2] - points[0]);

            Vector3 normal = Vector3.Normalize(Vector3.Cross(U, V));

            float dot = Vector3.Dot(camera, normal);

            double angle = Math.Acos(dot / (normal.Length() * camera.Length()));

            return angle > Math.PI / 2;
        }




        public void Draw(Graphics g, float angle)
        {
            float cos = (float)Math.Cos(angle * (Math.PI / 180));
            float sin = (float)Math.Sin(angle * (Math.PI / 180));

            CameraPos = new Vector3
            {
                X = half + 100 * sin,
                Y = 0,
                Z = half + 100 * cos
            };

            CamerDir = new Vector3
            {
                X = half - CameraPos.X,
                Y = Scale / 2 - CameraPos.Y,
                Z = half - CameraPos.Z
            };

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
                    corners = cornerCalc(Vectors, cos, sin);

                    float avg1 = (heightmap[i, j] + heightmap[i + 1, j] + heightmap[i, j + 1]) / 3;
                    float avg2 = (heightmap[i + 1, j + 1] + heightmap[i + 1, j] + heightmap[i, j + 1]) / 3;

                    b.Color = Colours[(int)(49 * Math.Min(1, Math.Max(0, avg1)))];
                    if (BackFace(new Vector3[3] { Vectors[2], Vectors[0], Vectors[1] }))
                        g.FillPolygon(b, new Point[3] { corners[2], corners[0], corners[1] });

                    b.Color = Colours[(int)(49 * Math.Min(1, Math.Max(0, avg2)))];
                    if (BackFace(new Vector3[3] { Vectors[2], Vectors[1], Vectors[3] }))
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
