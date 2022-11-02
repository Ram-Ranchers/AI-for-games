using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace TerrainGeneration
{
    public class NoiseManager : MonoBehaviour
    {
        public RawImage noiseTextureImage;
        public Terrain noiseTerrain;
        
        public int width = 256;
        public int height = 256;

        private string[] _noiseTypes;
        private int _currentNoiseIndex, _lastNoiseIndex;
        private Noise _noise;
        private float _scale, _lastScale;
        private int _seed, _lastSeed;
        
        private void Awake()
        {
            _noiseTypes = Noise.NoiseTypes.Keys.ToArray();

            _currentNoiseIndex = 0;
            _scale = 0.1f;
            _seed = 8;
            RecomputeNoise();
        }

        private void RecomputeNoise()
        {
            System.Type noiseClass = Noise.NoiseTypes[_noiseTypes[_currentNoiseIndex]];
            _noise = (Noise)System.Activator.CreateInstance(noiseClass);
            _noise.Seed = _seed;
            float[,] noise = new float[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    noise[y, x] = _noise.GetNoiseMap(x, y, _scale);
                }
            }
            
            SetNoiseTexture(noise);
        }
        
        private void SetNoiseTexture(float[,] noise)
        {
            Color[] pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[x + width * y] = Color.Lerp(Color.black, Color.white, noise[y, x]);
                }
            }
            
            Texture2D texture2D = new Texture2D(width, height);
            texture2D.SetPixels(pixels);
            texture2D.Apply();
            noiseTextureImage.texture = texture2D;
            noiseTerrain.terrainData.SetHeights(0, 0, noise);
        }

        private void UpdateUI()
        {
            if(_scale == _lastScale && _seed == _lastSeed && _currentNoiseIndex == _lastNoiseIndex)
            {
                return;
            }
            
            RecomputeNoise();

            _lastScale = _scale;
            _lastSeed = _seed;
            _lastNoiseIndex = _currentNoiseIndex;
        }
        
        private void OnGUI()
        {
            _currentNoiseIndex = GUI.SelectionGrid(new Rect(0f, 0f, 100f, _noiseTypes.Length * 25f), 
                _currentNoiseIndex, _noiseTypes, 1);
            
            _scale = GUI.HorizontalSlider(new Rect(120f, 0f, 100f, 20f), 
                _scale, 0.01f, 0.3f);

            _seed = (int)GUI.HorizontalSlider(new Rect(120f, 30f, 100f, 20f), 
                _seed, 0, 100);
            
            if(GUI.changed)
            {
                UpdateUI();
            }
        }
    }
}
