using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PortalRP
{
	public static class StaticVariables
	{
		public static Vector3 bluePortalPosition = new Vector3(0, 2, -5);
		public static Quaternion bluePortalRotation = Quaternion.Euler(0, 0, 0);
		public static Vector3 bluePortalScale = Vector3.one;

		public static Matrix4x4 bluePortalMatrix { get { return Matrix4x4.TRS(bluePortalPosition, bluePortalRotation, bluePortalScale); } }
        public static float bluePortalUniformScale { get { return (bluePortalScale.x + bluePortalScale.y) * 0.5f; } set { bluePortalScale.x = value; bluePortalScale.y = value; bluePortalScale.z = 1; } }



        public static Vector3 orangePortalPosition = new Vector3(0, 2, 5);
		public static Quaternion orangePortalRotation = Quaternion.Euler(0, 180, 0);
		public static Vector3 orangePortalScale = Vector3.one;

        public static Matrix4x4 orangePortalMatrix { get { return Matrix4x4.TRS(orangePortalPosition, orangePortalRotation, orangePortalScale); } }
        public static float orangePortalUniformScale { get { return (orangePortalScale.x + orangePortalScale.y) * 0.5f; } set { orangePortalScale.x = value; orangePortalScale.y = value; orangePortalScale.z = 1; } }



        public static Matrix4x4[] instances = new Matrix4x4[]
        {
            Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, Vector3.one),
            Matrix4x4.TRS(new Vector3(10, 0, 0), Quaternion.identity, Vector3.one),
            Matrix4x4.TRS(new Vector3(0, 0, 10), Quaternion.identity, Vector3.one),
            Matrix4x4.TRS(new Vector3(10, 0, 10), Quaternion.identity, Vector3.one),
            Matrix4x4.TRS(new Vector3(0, 10, 0), Quaternion.identity, Vector3.one),
            Matrix4x4.TRS(new Vector3(10, 10, 0), Quaternion.identity, Vector3.one),
            Matrix4x4.TRS(new Vector3(0, 10, 10), Quaternion.identity, Vector3.one),
            Matrix4x4.TRS(new Vector3(10, 10, 10), Quaternion.identity, Vector3.one),
        };
    }
}