using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PortalRP
{
	internal class PortalPipeline : RenderPipeline
	{
		public static ShaderTagId ShaderTagId = new ShaderTagId("SRPDefaultUnlit");

		private PortalRenderer renderer;

		public PortalPipeline(PortalAsset Asset)
		{
			renderer = new PortalRenderer(Asset.clearColor, Asset.mesh, Asset.material);
		}

		protected override void Render(ScriptableRenderContext Context, Camera[] Cameras)
		{
#if UNITY_EDITOR
			Debug.LogWarning("Rendering with depricated method!");
#endif
			Render(Context, new List<Camera>(Cameras));
		}

		protected override void Render(ScriptableRenderContext Context, List<Camera> Cameras)
		{
			//XRSystem.SetDisplayMSAASamples(MSAASamples.MSAA4x);

			// SortCameras(cameras); Sort cameras by depth

			for (int i = 0; i < Cameras.Count; i++)
			{
				renderer.RenderCamera(ref Context, Cameras[i]);
			}
		}
	}
}