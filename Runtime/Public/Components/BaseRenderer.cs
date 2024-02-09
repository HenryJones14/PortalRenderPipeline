using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

namespace PortalRP
{
	[DisallowMultipleComponent, Icon("Packages/com.gameboxinteractive.portalrp/Assets/Icons/BaseRenderer.png")]
	public abstract class BaseRenderer : MonoBehaviour
	{
		[field: SerializeField] public Material material { get; protected set; }
		[field: SerializeField] public Bounds bounds { get; protected set; }

		public abstract void Render(ref CommandBuffer Buffer);
		public abstract bool Cull(ref Plane[] ViewFrustum);
	}
}
