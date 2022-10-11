using System;
using System.Collections.Generic;

namespace TerrainGeneration
{
    public abstract class Noise
    {
        public static readonly Dictionary<string, Type> NoiseTypes = new Dictionary<string, Type>()
        {
            { "Perlin", typeof(PerlinNoise) },
            { "White", typeof(WhiteNoise) }
        };

        public int Seed;
        
        public abstract float GetNoiseMap(float x, float y, float scale = 1f);
    }
}
