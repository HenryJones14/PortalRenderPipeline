using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEditor.Rendering.CameraUI;
using System.Linq;
using UnityEditor.Rendering;

namespace PortalRP
{
    [CustomEditorForRenderPipeline(typeof(Camera), typeof(PortalAsset))]
    public class CustomCameraEditor : CameraEditor
    {
        private Camera script;
        private Camera.FieldOfViewAxis fovAxisMode;

        public override void OnInspectorGUI()
        {
            script = target as Camera;

            script.clearFlags = CameraClearFlags.Color;
            script.backgroundColor = PortalAsset.EditorInstance.clearColor;


            GUI.enabled = false;
            settings.DrawClearFlags();
            settings.DrawBackgroundColor();
            GUI.enabled = true;
            DrawCullingMask();
            EditorGUILayout.Space();
            DrawProjection();
            EditorGUILayout.Space();
            DrawClippingPlanes();
            GUI.enabled = false;
            settings.DrawNormalizedViewPort();
            EditorGUILayout.Space();
            settings.DrawDepth();
            settings.DrawRenderingPath();
            settings.DrawTargetTexture(false);
            settings.DrawOcclusionCulling();
            settings.DrawHDR();
            settings.DrawMSAA();
            settings.DrawDynamicResolution();
            settings.DrawMultiDisplay();
            settings.DrawTargetEye();
            settings.DrawVR();
            GUI.enabled = true;
        }

        private void DrawCullingMask()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_CullingMask"));
        }

        private void DrawProjection()
        {
            script.orthographic = (ProjectionType)EditorGUILayout.EnumPopup("Projection", script.orthographic ? ProjectionType.Orthographic : ProjectionType.Perspective) == ProjectionType.Orthographic;

            if (script.orthographic)
            {
                script.orthographicSize = EditorGUILayout.FloatField("Size", script.orthographicSize);
            }
            else
            {
                fovAxisMode = (Camera.FieldOfViewAxis)EditorGUILayout.EnumPopup("FOV Axis", fovAxisMode);

                if (fovAxisMode == Camera.FieldOfViewAxis.Horizontal)
                {
                    float aspectRatio = script.usePhysicalProperties ? script.sensorSize.x / script.sensorSize.y : script.aspect;
                    script.fieldOfView = Camera.HorizontalToVerticalFieldOfView(script.fieldOfView = EditorGUILayout.Slider("Field of View", Camera.VerticalToHorizontalFieldOfView(script.fieldOfView, aspectRatio), 1, 179), aspectRatio);
                }
                else
                {
                    script.fieldOfView = EditorGUILayout.Slider("Field of View", script.fieldOfView, 1, 179);
                }

                GUI.enabled = false;
                EditorGUILayout.Toggle("Physical Camera", script.usePhysicalProperties);
                script.usePhysicalProperties = false;
                GUI.enabled = true;
            }
        }

        private void DrawClippingPlanes()
        {
            Rect rect, label, content;

            // Near plane
            EditorGUILayout.LabelField("Clipping Planes");
            rect = GUILayoutUtility.GetLastRect();

            rect.width -= EditorGUIUtility.labelWidth;
            rect.x += EditorGUIUtility.labelWidth;

            label = new Rect(rect.x, rect.y, 38, rect.height);
            content = new Rect(rect.x + label.width, rect.y, rect.width - label.width, rect.height); ;

            EditorGUI.PrefixLabel(label, new GUIContent("Near"));
            EditorGUI.FloatField(content, 1);

            // Far plane
            EditorGUILayout.LabelField("");
            rect = GUILayoutUtility.GetLastRect();

            rect.width -= EditorGUIUtility.labelWidth;
            rect.x += EditorGUIUtility.labelWidth;

            label = new Rect(rect.x, rect.y, 38, rect.height);
            content = new Rect(rect.x + label.width, rect.y, rect.width - label.width, rect.height); ;

            EditorGUI.PrefixLabel(label, new GUIContent("Far"));
            EditorGUI.FloatField(content, 1);
        }
    }
}