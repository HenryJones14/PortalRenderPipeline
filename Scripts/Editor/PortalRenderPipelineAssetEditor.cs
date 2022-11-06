using UnityEngine.Rendering;
using UnityEditor;
using UnityEngine;

namespace PortalRP.Core
{
    [CustomEditor(typeof(PortalRenderPipelineAsset))]
    public class PortalRenderPipelineAssetEditor : Editor
    {
        PortalRenderPipelineAsset script;

        public void OnEnable()
        {
            script = (PortalRenderPipelineAsset)target;
        }

        public void OnDisable()
        {
            script = null;
        }

        public override void OnInspectorGUI() // EditorGUILayout
        {

        }
    }
}
