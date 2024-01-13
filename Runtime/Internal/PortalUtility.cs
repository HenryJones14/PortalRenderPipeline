using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PortalRP
{
	internal static class PortalUtility
	{
		public static Matrix4x4 CalculateObliqueMatrix(Camera Camera, Transform Plane)
		{
			//float dot = sign(Vector3.Dot(_plane.forward, _plane.position - _camera.transform.position));
			/*
				if (a > 0.0f) return 1.0f;
				if (a < 0.0f) return -1.0f;
				return 0.0f;
			*/

			Vector3 cameraSpacePosition = Camera.worldToCameraMatrix.MultiplyPoint(Plane.position);
			Vector3 cameraSpaceNormal = Camera.worldToCameraMatrix.MultiplyVector(Plane.forward);

			float cameraSpaceDistance = -Vector3.Dot(cameraSpacePosition, cameraSpaceNormal);

			Vector4 CameraSpaceClipPlane = cameraSpaceNormal;
			CameraSpaceClipPlane.w = cameraSpaceDistance;

			return Camera.CalculateObliqueMatrix(CameraSpaceClipPlane);
		}
	}
}