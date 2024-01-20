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
			renderer = new PortalRenderer(Asset);
		}

		protected override void Render(ScriptableRenderContext Context, Camera[] Cameras)
		{
			Debug.LogWarning("Rendering with depricated method!");
			Render(Context, new List<Camera>(Cameras));
		}

		protected override void Render(ScriptableRenderContext Context, List<Camera> Cameras)
		{
			for (int i = 0; i < Cameras.Count; i++)
			{
				renderer.RenderCamera(ref Context, Cameras[i]);
			}
		}
	}
}