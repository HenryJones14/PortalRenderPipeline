using UnityEngine.Rendering;
using UnityEngine;

namespace PortalRP.Core
{
    public class PortalRenderPipelineRenderer : RenderPipeline
    {
        //private CommandBuffer CommandBuffer = new CommandBuffer() { name = "Init" };

        protected override void Render(ScriptableRenderContext Context, Camera[] Cameras)
        {
            foreach (Camera camera in Cameras)
            {
                RenderCamera(camera, Context);
            }
        }

        protected void RenderCamera(Camera CameraToRender, ScriptableRenderContext Context)
        {
            Context.SetupCameraProperties(CameraToRender, CameraToRender.stereoEnabled);

            if (CameraToRender.stereoEnabled)
            {
                Context.StartMultiEye(CameraToRender);
            }

            Context.DrawSkybox(CameraToRender);

            if (CameraToRender.stereoEnabled)
            {
                Context.StopMultiEye(CameraToRender);
            }

            Context.Submit();
        }
    }
}
