using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("PortalRP.Editor")]
namespace PortalRP
{
	[CreateAssetMenu(menuName = "Rendering/PortalRP/PortalRP Asset", fileName = "PRP_Asset")]
	internal class PortalAsset : RenderPipelineAsset
	{
#if UNITY_EDITOR
		public static PortalAsset EditorInstance;
#endif

		public Color clearColor = Color.clear;

		public Mesh mesh;
		public Mesh teapot;

        public Material material;

		protected override RenderPipeline CreatePipeline()
		{
#if UNITY_EDITOR
			EditorInstance = this;
#endif
			return new PortalPipeline(this);
		}
	}
}