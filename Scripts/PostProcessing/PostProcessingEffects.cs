using UnityEngine;

namespace PostProcessing
{
    public class PostProcessingEffects : MonoBehaviour
    {
        public Material oceanEffect;

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, oceanEffect);
        }
    }
}
