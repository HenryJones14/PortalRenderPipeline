using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PortalRP
{
	public static class StaticVariables
	{
		public static Vector3 bluePortalPosition = new Vector3(0, 2, -5);
		public static Quaternion bluePortalRotation = Quaternion.Euler(0, 0, 0);
		public static float bluePortalScale = 1;

		public static Matrix4x4 bluePortalMatrix { get { return Matrix4x4.TRS(bluePortalPosition, bluePortalRotation, new Vector3(bluePortalScale, bluePortalScale, 1)); } }



		public static Vector3 orangePortalPosition = new Vector3(0, 2, 5);
		public static Quaternion orangePortalRotation = Quaternion.Euler(0, 180, 0);
		public static float orangePortalScale = 1;

        public static Matrix4x4 orangePortalMatrix { get { return Matrix4x4.TRS(orangePortalPosition, orangePortalRotation, new Vector3(orangePortalScale, orangePortalScale, 1)); } }
    }
}