using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

namespace PortalRP
{
	[Icon("Packages/com.gameboxinteractive.portalrp/Assets/Icons/SingleRenderer.png")]
	public class SingleRenderer : BaseRenderer
	{
#if UNITY_EDITOR
		public ref MeshInstance mesh { get { return ref _mesh; } }
		[SerializeField] protected MeshInstance _mesh;
#endif
		[field: SerializeField] public int submeshIndex { get; protected set; }

		private Mesh renderMesh;

		public override bool Cull(ref Plane[] ViewFrustum)
		{
			if (renderMesh == null || material == null)
			{
				return false;
			}
			bounds = new Bounds(transform.position, Vector3.one);
			return GeometryUtility.TestPlanesAABB(ViewFrustum, bounds);
		}

		public override void Render(ref CommandBuffer Buffer)
		{
			Buffer.DrawMesh(renderMesh, transform.localToWorldMatrix, material, submeshIndex, -1, null);
		}
	}
}