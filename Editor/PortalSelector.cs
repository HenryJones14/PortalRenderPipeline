using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace PortalRP
{
    public static class PortalSelector
    {
        //[InitializeOnLoadMethod()]
        public static void Initialize()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public static void OnSceneGUI(SceneView SceneView)
        {
            string tool = ToolManager.activeToolType.ToString();
            float blue = Vector3.Distance(SceneView.camera.transform.position, StaticVariables.bluePortalPosition);
            float orange = Vector3.Distance(SceneView.camera.transform.position, StaticVariables.orangePortalPosition);

            Handles.PositionHandleIds bluePositionHandle = Handles.PositionHandleIds.@default;
            Handles.RotationHandleIds blueRotationHandle = Handles.RotationHandleIds.@default;

            Handles.PositionHandleIds orangePositionHandle = Handles.PositionHandleIds.@default;
            Handles.RotationHandleIds orangeRotationHandle = Handles.RotationHandleIds.@default;

            if (tool == "UnityEditor.MoveTool")
            {
                if(blue < 7 || TestHandle(GUIUtility.hotControl, bluePositionHandle))
                {
                    StaticVariables.bluePortalPosition = Handles.PositionHandle(bluePositionHandle, StaticVariables.bluePortalPosition, StaticVariables.bluePortalRotation);
                }

                if (orange < 7 || TestHandle(GUIUtility.hotControl, orangePositionHandle))
                {
                    StaticVariables.orangePortalPosition = Handles.PositionHandle(orangePositionHandle, StaticVariables.orangePortalPosition, StaticVariables.orangePortalRotation);
                }
            }
            if (tool == "UnityEditor.RotateTool")
            {
                if (blue < 7 || TestHandle(GUIUtility.hotControl, blueRotationHandle))
                {
                    StaticVariables.bluePortalRotation = Handles.RotationHandle(blueRotationHandle, StaticVariables.bluePortalRotation, StaticVariables.bluePortalPosition);
                }

                if (orange < 7 || TestHandle(GUIUtility.hotControl, orangeRotationHandle))
                {
                    StaticVariables.orangePortalRotation = Handles.RotationHandle(orangeRotationHandle, StaticVariables.orangePortalRotation, StaticVariables.orangePortalPosition);
                }
            }
            if (tool == "UnityEditor.ScaleTool")
            {
                if (blue < 7)
                {
                    StaticVariables.bluePortalScale = Handles.ScaleHandle(StaticVariables.bluePortalScale, StaticVariables.bluePortalPosition, StaticVariables.bluePortalRotation, HandleUtility.GetHandleSize(StaticVariables.bluePortalPosition));
                }

                if (orange < 7)
                {
                    StaticVariables.orangePortalScale = Handles.ScaleHandle(StaticVariables.orangePortalScale, StaticVariables.orangePortalPosition, StaticVariables.orangePortalRotation, HandleUtility.GetHandleSize(StaticVariables.orangePortalPosition));
                }
            }
        }

        private static bool TestHandle(int HotControlID, Handles.PositionHandleIds IDs)
        {
            if (HotControlID == IDs.x || HotControlID == IDs.xy || HotControlID == IDs.xyz)
            {
                return true;
            }

            if (HotControlID == IDs.y || HotControlID == IDs.yz)
            {
                return true;
            }

            if (HotControlID == IDs.z)
            {
                return true;
            }

            return false;
        }

        private static bool TestHandle(int HotControlID, Handles.RotationHandleIds IDs)
        {
            if (HotControlID == IDs.x || HotControlID == IDs.xyz)
            {
                return true;
            }

            if (HotControlID == IDs.y)
            {
                return true;
            }

            if (HotControlID == IDs.z)
            {
                return true;
            }

            return false;
        }
    }
}
