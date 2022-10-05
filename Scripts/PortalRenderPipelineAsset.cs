using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PortalRP.Core
{
    [CreateAssetMenu(menuName = "Rendering/PortalRP Asset")]
    public internal class PortalRenderPipelineAsset : RenderPipelineAsset
    {
        protected override RenderPipeline CreatePipeline()
        {
            return null;
        }
    }
}