using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PortalRP
{
	[CreateAssetMenu(menuName = "Rendering/PortalRP/PortalRP Asset", fileName = "PRP_Asset")]
	internal class PortalAsset : RenderPipelineAsset
	{
		public Color clearColor = Color.clear;

		public Mesh mesh;
		public Material material;

		protected override RenderPipeline CreatePipeline()
		{
			return new PortalPipeline(this);
		}
	}
}