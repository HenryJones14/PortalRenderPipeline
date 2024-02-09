using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

namespace PortalRP
{
	[Icon("Packages/com.gameboxinteractive.portalrp/Assets/Icons/CombinedRenderer.png")]
	public class CombinedRenderer : BaseRenderer
	{
#if UNITY_EDITOR
		[field: SerializeField] public MeshInstance mesh { get; protected set; }
#endif

		public override bool Cull(ref Plane[] ViewFrustum)
		{
			throw new System.NotImplementedException();
		}

		public override void Render(ref CommandBuffer Buffer)
		{
			throw new System.NotImplementedException();
		}
	}
}
