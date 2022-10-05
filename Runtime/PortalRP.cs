using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.Universal
{
    internal class ClipPlane
    {
        internal Vector3 Normal;
        internal Vector3 Position;

        internal ClipPlane(Vector3 NewNormal, Vector3 NewPosition)
        {
            Normal = NewNormal;
            Position = NewPosition;
        }

        internal Vector4 GetClipPlane()
        {
            return new Vector4(Normal.x, Normal.y, Normal.z, Vector3.Dot(Position, -Normal));
        }
    }
}