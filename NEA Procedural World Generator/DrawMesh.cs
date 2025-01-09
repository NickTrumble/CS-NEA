using System;
using System.Drawing;

namespace NEA_Procedural_World_Generator
{
    class DrawMesh
    {
        public float[,] heightmap { get; set; }
        public int xoffset { get; set; }
        public int yoffset { get; set; }
        public int tilewidth { get; set; }
        public int tileheight { get; set; }
        public int scale { get; set; }
        public int half { get; set; }
        public DrawMesh(float[,] inTerrain, int xo, int yo, int s)
        {
            heightmap = inTerrain;
            xoffset = xo;
            yoffset = yo;
            tilewidth = 1000 / heightmap.GetLength(0);
            tileheight = 800 / heightmap.GetLength(1);
            half = heightmap.GetLength(0) / 2;
            scale = s;
        }

        public Point PointCalc(int x, int y, float z, float cos, float sin)
        {
            int cx = x - half;
            int cy = y - half;
            float hegiht = z * scale;
            //step back
            //int X = (int)((cos * x - cos *y) * tilewidth / 2f + xoffset);
            //int Y = (int)((cos *x +  cos * y) * tileheight / 2f - z * scale + yoffset);

            int X = (int)((cos * cx - sin * cy) * tilewidth / 2f + xoffset);
            int Y = (int)((sin * cx + cos * cy) * tilewidth / 4f - hegiht + yoffset + half);

            return new Point(X, Y);
        }

        public Color GetColour(int elevation)
        {
            int intensity = Math.Min(127, elevation * 13);
            return Color.FromArgb(2 * intensity, 2 * intensity, 128 + intensity);
        }

        public void Draw(Graphics g)
        {
            Color[] Colours = new Color[50];
            float cos = (float)Math.Cos((Math.PI / 2) * (Math.PI / 180));
            float sin = (float)Math.Sin((Math.PI / 2) * (Math.PI / 180));
            int size = heightmap.GetLength(0);
            Point[] corners1 = new Point[3];
            Point[] corners2 = new Point[3];
            SolidBrush b = new SolidBrush(Color.White);
            SolidBrush b2 = new SolidBrush(Color.White);
            int start = (1 >= 180) ? size - 2 : 0;
            int end = (start == 0) ? size - 2 : 0;
            int step = (start == 0) ? 1 : -1;
            //int start = 0;
            //int end = size - 2;
            //int step = 1;
            for (int i = start; i != end; i += step)
            {
                for (int j = start; j != end; j += step)
                {
                    corners1[0] = PointCalc(i, j, heightmap[i, j], cos, sin);
                    corners1[1] = PointCalc(i, j + 1, heightmap[i, j + 1], cos, sin);
                    corners1[2] = PointCalc(i + 1, j, heightmap[i + 1, j], cos, sin);
                    float avgh = (heightmap[i, j] + heightmap[i + 1, j] + heightmap[i, j + 1]) / 3;
                    b.Color = Colours[(int)(49 * Math.Max(0, avgh))];

                    g.FillPolygon(b, corners1);
                    corners2[0] = PointCalc(i + 1, j + 1, heightmap[i + 1, j + 1], cos, sin);
                    corners2[1] = PointCalc(i, j + 1, heightmap[i, j + 1], cos, sin);
                    corners2[2] = PointCalc(i + 1, j, heightmap[i + 1, j], cos, sin);

                    float avgh2 = (heightmap[i + 1, j + 1] + heightmap[i + 1, j] + heightmap[i, j + 1]) / 3;
                    b2.Color = Colours[(int)(4 * Math.Max(0, avgh2))];

                    g.FillPolygon(b2, corners2);
                }
            }
        }
    }
}
//GOALS
//mesh
//mass optimisation
//save options e.g. other formats plus select region
//manual and settings:
//  instructions
//  cmaps and colour blind
//  font sizes

//loading bar/symbol / make saving async
//terracingh
//zooming
//smaller worlds
//fix ui design