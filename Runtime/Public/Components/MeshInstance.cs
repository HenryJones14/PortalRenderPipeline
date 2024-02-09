using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PortalRP
{
#if UNITY_EDITOR
	[DisallowMultipleComponent, ExecuteInEditMode, Icon("Packages/com.gameboxinteractive.portalrp/Assets/Icons/MeshInstance.png")]
	public class MeshInstance : MonoBehaviour
	{
		public Mesh mesh;

		public void Update()
		{
			hideFlags = HideFlags.DontSaveInBuild;
		}
	}
#endif
}