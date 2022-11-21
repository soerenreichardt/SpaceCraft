using UnityEngine;

namespace Terrain.Color
{
    public class ColorGenerator
    {
        private const int TEXTURE_RESOLUTION = 50;

        private static readonly int TerrainColors = Shader.PropertyToID("_TerrainColors");
        
        private readonly ColorSettings colorSettings;
        private readonly Texture2D gradientTexture;

        public ColorGenerator(ColorSettings colorSettings)
        {
            this.colorSettings = colorSettings;
            this.gradientTexture = new Texture2D(TEXTURE_RESOLUTION, 1);
            UpdateColors();
        }

        public void UpdateColors()
        {
            for (int i = 0; i < TEXTURE_RESOLUTION; i++)
            {
                var color = colorSettings.gradient.Evaluate((float) i / TEXTURE_RESOLUTION);
                gradientTexture.SetPixel(i, 0, color);
            }
            gradientTexture.Apply();
            colorSettings.material.SetTexture(TerrainColors, gradientTexture);
        }
    }
}
