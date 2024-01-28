using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;


namespace PortalRP
{
	internal class PortalPipeline : RenderPipeline
	{
		private CameraRenderer renderer;

		public PortalPipeline(PortalAsset Asset)
		{
			renderer = new CameraRenderer(Asset);
		}

		protected override void Render(ScriptableRenderContext Context, Camera[] Cameras)
		{
			Debug.LogWarning("Rendering with depricated method!");
			renderer.RenderAllCameras(ref Context, new List<Camera>(Cameras));
		}

		protected override void Render(ScriptableRenderContext Context, List<Camera> Cameras)
		{
			renderer.RenderAllCameras(ref Context, Cameras);
		}
	}
}