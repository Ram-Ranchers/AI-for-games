using UnityEngine;

namespace TerrainGeneration
{
    public class WhiteNoise : Noise
    {
        public override float GetNoiseMap(float x, float y, float scale = 1)
        {
            return Random.Range(0f, 1f);
        }
    }
}
