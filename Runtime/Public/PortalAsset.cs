using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PortalRP
{
	[CreateAssetMenu(menuName = "Rendering/PortalRP/PortalRP Asset", fileName = "PRP_Asset")]
	public class PortalAsset : RenderPipelineAsset
	{
#if UNITY_EDITOR
		public static PortalAsset EditorInstance;
#endif

		public Color clearColor = Color.clear;

		public Mesh portalMesh;
		public Mesh teapotMesh;

		public Material portalMaterial;
		public Material teapotMaterial;

		protected override RenderPipeline CreatePipeline()
		{
#if UNITY_EDITOR
			EditorInstance = this;
#endif
			return new PortalPipeline(this);
		}
	}
}