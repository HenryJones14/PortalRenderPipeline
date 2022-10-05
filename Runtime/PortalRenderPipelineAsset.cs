using UnityEngine.Rendering;
using UnityEngine;

namespace PortalRP.Core
{
    [CreateAssetMenu(fileName = "Asset", menuName = "Rendering/Asset")]
    public class PortalRenderPipelineAsset : RenderPipelineAsset
    {
        protected override RenderPipeline CreatePipeline()
        {
            return new PortalRenderPipelineRenderer();
        }
    }
}