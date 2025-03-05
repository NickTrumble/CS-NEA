using System;

namespace NEA_Procedural_World_Generator
{
    //class to generate the perlin noise used to make the noisemap : basenoise
    public class PerlinNoise : BaseNoise
    {

        //constructor taking noiise parameters
        public PerlinNoise(int size, int octave, float pers) : base(size, octave, pers) { }

        //smoothens transitions between gradient vectors
        static float Fade_function(float f) => f * f * f * (f * (f * 6 - 15) + 10);//blend

        // linear interpolation between a and b, with interpolation value = t
        public static float Lerp_function(float f, float a, float b) => a + f * (b - a);//interpolation


        //generates a single octave of the perlin noise to be combined into multiple
        public override float Single_octave(float x, float y)
        {
            int xint = fastfloor(x);//x rounded down
            int yint = fastfloor(y);//y rounded down

            float xfloat = x - xint;//part of x after decimal point
            float yfloat = y - yint;//part of y after decimal point

            float i = Fade_function(xfloat);//smoothened out version of xfloat
            float j = Fade_function(yfloat);//smoothened out version of yfloat

            int tl = Ptable[(Ptable[xint] + yint)];//random grandient from bottom left corner
            int tr = Ptable[(Ptable[(xint + 1)] + yint)];//random gradient from bottom right corner
            int bl = Ptable[(Ptable[xint] + (yint + 1))];//random gradient from top left corner
            int br = Ptable[(Ptable[(xint + 1)] + (yint + 1))];//random gradient from top right corner

            //linear interpolation between the dot product of the two vectors at the corners, using the smoothened xfloat
            //value as the interpolation value
            float x1 = Lerp_function(i, Gradient_calc(tl, xfloat, yfloat), Gradient_calc(tr, xfloat - 1, yfloat));
            float x2 = Lerp_function(i, Gradient_calc(bl, xfloat, yfloat - 1), Gradient_calc(br, xfloat - 1, yfloat - 1));

            //more linear interpolation to smooth it out even more using the smoothened yfloat as interpolation value
            return Lerp_function(j, x1, x2);
        }
    }

    //class to generate simplex noise used to make the noisemap : basenoise
    public class SimplexNoise : BaseNoise
    {
        public float unskew = (3 - (float)Math.Sqrt(3)) / 6;// converts skewed coords to normal
        public float skew = 0.5f * (float)(Math.Sqrt(3) - 1);
        public SimplexNoise(int size, int octave, float pers) : base(size, octave, pers) { }

        public override float Single_octave(float xin, float yin)
        {
            /*skew matrix = [
             *  [ 1 + Skew Factor, Skew Factor]
             *  [ Skew Factor, 1 + Skew Factor]
             * ]
             * 
             * Inverse Skew Matrix(unskew matrix) = [
             *  [ 1 - Unskew Factor, - Unskew Factor]
             *  [ - Unskew Factor, 1 - Unskew Factor]
             * ]
            */
            float skew_factor = (xin + yin) * skew;
            int i = fastfloor(xin + skew_factor);
            int j = fastfloor(yin + skew_factor);

            //multiply by linear unskew matrix to transform skewed grid back to normal coordinates

            float unskew_factor = (i + j) * unskew;
            float x = xin - (i - unskew_factor);//calculate distance from x and y to the cell origin(origin corner)
            float y = yin - (j - unskew_factor);//aka distance to the different axis

            int ioffset, joffset;
            // square cut diagonaly, bl to tr 
            if (x > y)// if closer to the x axis, in the bottom triangle else in the top treiangle
            {
                //if in bottom, next corner is br, then tr
                ioffset = 1;
                joffset = 0;
            }
            else
            {
                //if in top, next corner is tl, then tr
                ioffset = 0;
                joffset = 1;
            }

            int ii = i & 255;// Logical and function = faster version of the modulus operation
            int jj = j & 255;// only works when the modulus max is a power of 2

            //all values are unskewed, and 2 * unskew represents unskewing 2 units across
            //the distances include negatives, as they include directions,aka they are displacements

            //coordinate - offset + unskew matrix * offset
            float middlex = x - ioffset + unskew; // move from xdis to xdis + ioffset, moving to the next corner
            float middley = y - joffset + unskew; // move from ydis to ydis + joffset, moving to the next corner
            float finalx = x - 1f + 2f * unskew; // mover from xdis to xdis + 1, moving to the final corner
            float finaly = y - 1f + 2f * unskew; // mover from ydis to ydis + 1, moving to the final corner

            //calculate contributiuons from each corner
            float n1 = Corner_contribution(x, y, ii + Ptable[jj]);//contribution from corner 1
            float n2 = Corner_contribution(middlex, middley, ii + ioffset + Ptable[jj + joffset]); //contribution from corner 2
            float n3 = Corner_contribution(finalx, finaly, ii + 1 + Ptable[jj + 1]);// contribution from corner 3

            return (n1 + n2 + n3) * 40f;//return total cointribution
        }

        public float Corner_contribution(float x, float y, int index)
        {
            float t = 0.5f - x * x - y * y;// radius of influence from center point of triangle - distance to point (x^2 + y^2)
            if (t < 0f)
            {
                return 0f;//if distance oiut of radius for first corner, contribution = 0
            }
            else
            {
                t *= t;//apply a smooth fall off
                return t * t * Gradient_calc(Ptable[index], x, y);
                // return distance to center ^4 * random gradient
            }
        }
    }

    //basic nosie class - holds functions needed in both simplex and perlin
    public class BaseNoise
    {
        public static int[] Ptable { get; set; }
        public static int Num_samples { get; set; }//size
        public static int octaves { get; set; }
        public static float persistance { get; set; }

        //array for gradiant vectors
        public static float[][] gradients =
        {
            new float[] { 1, 1 }, new float[] { -1, 1 }, new float[] { 1, -1 }, new float[] { -1, -1 },
            new float[] { 1, 0 }, new float[] { -1, 0 }, new float[] { 0, 1 }, new float[] { 0, -1 }
        };

        public BaseNoise(int size, int octave, float pers)//constructor
        {
            Ptable = Permutation_Gen.Generation(size);
            Num_samples = size;
            octaves = octave;
            persistance = pers;
        }

        //Dot product between gradiant vector g and position vector (x, y)
        public static float Dot_product(float[] g, float x, float y) => g[0] * x + g[1] * y;

        //generates a gradiant using permutation table and gradianbts
        public static float Gradient_calc(float corner, float x, float y) => Dot_product(gradients[(int)corner % 8], x, y);

        //combines multiple octaves of perlin noise to generate the nosiemap
        public float Noise_method(float x, float y)
        {
            float amplitude = 1;//contains the highest posible noise value for each octave
            float frequency = 1;//increases the difference between each pixel in each octave, increasing uniquness
            float noise = 0;//cumulative generated noise
            float max = 0;//contains the cumulative maximum noise value before dividing
            for (int i = 0; i < octaves; i++)
            {
                noise += Single_octave(x * frequency, y * frequency) * amplitude;
                max += amplitude;//add on the highest posible outcome for this octave
                amplitude *= persistance;//decrease/increase the max noise value for this octave
                frequency *= 2f;//increase frequency for each octave
            }
            //return cumulative noise generated divided by the cumulative total posible 
            return noise / max;

        }
        //virtual function for other functions in this class to refer to
        public virtual float Single_octave(float x, float y) { return 0f; }

        //faster version of Math.floor
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