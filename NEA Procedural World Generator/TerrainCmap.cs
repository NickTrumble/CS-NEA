using System;
using System.Drawing;

namespace NEA_Procedural_World_Generator
{
    //generates a colourmap on the program
    public class TerrainCmap
    {
        //list of base collours used in the cmap
        public static Color[] cmapC =
        {
            Color.FromArgb(29, 116, 214), Color.FromArgb(45, 209, 111),
            Color.FromArgb(241, 252, 156), Color.FromArgb(128, 93, 87),
            Color.White
        };

        public static Color[,] ColourSchemes = new Color[4, 5]
        {
            {
                //lava Colour Scheme #0
                Color.Yellow, Color.Orange, Color.Red, Color.Brown, Color.Black
            },
            {
                //Purple Colour Scheme #1
                Color.Pink, Color.Violet, Color.Magenta, Color.DarkViolet, Color.Purple
            },
            {
                //Yellow Colour Scheme #2
                Color.LightYellow, Color.Khaki, Color.Yellow, Color.Goldenrod, Color.Peru
            },
            {
                //Natural Colour Scheme #3
                Color.DodgerBlue, Color.FromArgb(45, 209, 111), Color.Khaki, 
                Color.FromArgb(128, 93, 87), Color.White
            },
        };

        //interpolates between base colours in the cmap depending on the noise value
        public static Color Interpolate_value(float noise_value)
        {
            noise_value = (float)Math.Round(noise_value, 1);
            if (noise_value < 0.1) // waterd
            {
                return cmapC[0];
            }
            else if (noise_value < 0.3) // sansd
            {
                float t = (noise_value - 0.1f) / 0.2f;
                return Colour_lerp(cmapC[0], cmapC[1], t);
            }
            else if (noise_value < 0.5) // grass
            {
                float t = (noise_value - 0.3f) / 0.2f;
                return Colour_lerp(cmapC[1], cmapC[2], t);
            }
            else if (noise_value < 0.7) // rock
            {
                float t = (noise_value - 0.5f) / 0.2f;
                return Colour_lerp(cmapC[2], cmapC[3], t);
            }
            else // snow
            {
                float t = (noise_value - 0.7f) / 0.3f;
                return Colour_lerp(cmapC[3], cmapC[4], t);
            }


        }

        // linear interpolation betweeb two colours
        public static Color Colour_lerp(Color c1, Color c2, float interpolation_value)
        {
            interpolation_value = Math.Max(Math.Min(interpolation_value, 1), 0);

            int r = (int)((1 - interpolation_value) * c1.R + interpolation_value * c2.R);
            int g = (int)((1 - interpolation_value) * c1.G + interpolation_value * c2.G);
            int b = (int)((1 - interpolation_value) * c1.B + interpolation_value * c2.B);

            return Color.FromArgb(r, g, b);
        }
    }

}