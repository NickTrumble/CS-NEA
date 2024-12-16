using System;

namespace NEA_Procedural_World_Generator
{
    //add base class noise, inheritance, and simplex implimentation
    //class to generate the perlin noise used to make the noisemap
    public class PerlinNoise
    {
        public static int[] Ptable { get; set; }
        public static int Num_samples { get; set; }
        public static int octaves { get; set; }
        public static float persistance { get; set; }


        //array for gradiant vectors
        public static float[][] gradients =
        {
            new float[] { 1, 1 }, new float[] { -1, 1 }, new float[] { 1, -1 }, new float[] { -1, -1 },
            new float[] { 1, 0 }, new float[] { -1, 0 }, new float[] { 0, 1 }, new float[] { 0, -1 }
        };

        //constructor taking noiise parameters
        public PerlinNoise(int size, int octave, float pers)
        {
            Ptable = Permutation_Gen.Generation(size);
            Num_samples = size;
            octaves = octave;
            persistance = pers;
        }


        //combines multiple octaves of perlin noise to generate the nosiemap
        public float Noise_method(float x, float y)
        {
            float amplitude = 1;
            float frequency = 1;
            float noise = 0;
            float max = 0;
            for (int i = 0; i < octaves; i++)
            {
                noise += Single_octave(x * frequency, y * frequency) * amplitude;
                max += amplitude;
                amplitude *= persistance;
                frequency *= 2f;
            }
            return noise / max;

        }

        //Dot product between gradiant vector g and position vector (x, y)
        public static float Dot_product(float[] g, float x, float y) => g[0] * x + g[1] * y;

        //generates a gradiant using permutation table and gradianbts
        public static float Gradient_calc(float corner, float x, float y)
        {
            return Dot_product(gradients[(int)corner % gradients.Length], x, y);
        }

        //smoothens transitions between gradient vectors
        static float Fade_function(float f) => f * f * f * (f * (f * 6 - 15) + 10);//blend

        // linear interpolation between a and b, with interpolation value = t
        public static float Lerp_function(float f, float a, float b) => a + f * (b - a);//interpolation

        //generates a single octave of the perlin noise to be combined into multiple
        public float Single_octave(float x, float y)
        {
            int xint = (int)fastfloor(x);
            int yint = (int)fastfloor(y);

            float xfloat = x - xint;
            float yfloat = y - yint;

            float i = Fade_function(xfloat);
            float j = Fade_function(yfloat);

            int tl = Ptable[(Ptable[xint] + yint)];
            int tr = Ptable[(Ptable[(xint + 1)] + yint)];
            int bl = Ptable[(Ptable[xint] + (yint + 1))];
            int br = Ptable[(Ptable[(xint + 1)] + (yint + 1))];


            float x1 = Lerp_function(i, Gradient_calc(tl, xfloat, yfloat), Gradient_calc(tr, xfloat - 1, yfloat));
            float x2 = Lerp_function(i, Gradient_calc(bl, xfloat, yfloat - 1), Gradient_calc(br, xfloat - 1, yfloat - 1));

            return Lerp_function(j, x1, x2);
        }

        public static int fastfloor(float x) => (x >= 0) ? (int)x : (int)(x - 1);
    }

    //Generates the permutation table for Perlin/Simplex noise class
    public class Permutation_Gen
    {
        //shuffles the permutatiojn table using a fisher yates shuffle
        public static int[] Shuffle_table(int size, int[] table)
        {
            Random rnd = new Random(Environment.TickCount ^ DateTime.Now.Millisecond);
            for (int i = 0; i < size; i++)
            {
                int a = rnd.Next(size);
                int noise_bitmap = rnd.Next(size);

                int t = table[a];
                table[a] = table[noise_bitmap];
                table[noise_bitmap] = t;
            }
            return table;//swap random indicies of table with other randoms
        }

        //fills half an array with values 1 - size and then duplicates to the second hjalf
        public static int[] Generation(int size)
        {
            int[] table = new int[size * 2];
            for (int i = 0; i < size; i++)
            {
                table[i] = i;
                table[size + i] = i;
            } //initialise wiht values 1 to size - 1


            //return shuffled table
            return Shuffle_table(size, table);
        }
    }
}
