using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace PortalRP
{
	internal class PortalRenderer
	{
		// Public variables
		PortalAsset asset;

        // Private variables
        CommandBuffer buffer;


		public PortalRenderer(PortalAsset Asset)
        {
            asset = Asset;

            buffer = new CommandBuffer();
			buffer.name = "PortalRP";

			XRSystem.Initialize(Initialize, Shader.Find("Hidden/Universal Render Pipeline/XR/XROcclusionMesh"), Shader.Find("Hidden/Universal Render Pipeline/XR/XRMirrorView"));

			XRSystem.SetDisplayMSAASamples(MSAASamples.None);
			XRSystem.SetRenderScale(1f);
        }

		~PortalRenderer()
		{
			XRSystem.Dispose();

			buffer.Dispose();
			buffer = null;
		}

		private XRPass Initialize(XRPassCreateInfo CreateInfo)
		{
			return XRPass.CreateDefault(CreateInfo);
        }

        private void RenderOpaqueScene(ref ScriptableRenderContext RenderContext, Camera RenderCamera, ref ScriptableCullingParameters CullingParameters, ref RenderStateBlock StateBlock)
        {
            CullingResults results = RenderContext.Cull(ref CullingParameters);

            SortingSettings sorting = new SortingSettings(RenderCamera);
            sorting.criteria = SortingCriteria.CommonOpaque;

            DrawingSettings drawing = new DrawingSettings(new ShaderTagId("PortalRP"), sorting);
            FilteringSettings filtering = new FilteringSettings(RenderQueueRange.opaque);

            RenderContext.DrawRenderers(results, ref drawing, ref filtering, ref StateBlock);

            RenderContext.ExecuteCommandBuffer(buffer);
            buffer.Clear();
        }

        private void SceneTransparentScene()
        {

        }

        public void RenderCamera(ref ScriptableRenderContext Context, Camera RenderCamera)
        {
            bool mirror = false;

            if (RenderCamera.stereoEnabled)
            {
                XRLayout layout = XRSystem.NewLayout();
                layout.AddCamera(RenderCamera, RenderCamera);

                foreach ((Camera camera, XRPass xrpass) in layout.GetActivePasses())
                {
                    xrpass.StartSinglePass(buffer);

                    buffer.SetRenderTarget(xrpass.renderTarget, 0, CubemapFace.Unknown, -1);
                    buffer.ClearRenderTarget(RTClearFlags.All, asset.clearColor, 1f, 0u);

                    // GL.GetGPUProjectionMatrix(xrPass.GetProjMatrix(viewIndex), renderIntoTexture); * xrPass.GetViewMatrix(viewIndex)
                    buffer.SetGlobalMatrixArray("SterioViewMatrix", new Matrix4x4[] { xrpass.GetViewMatrix(0), xrpass.GetViewMatrix(1) });
                    buffer.SetGlobalMatrixArray("SterioProjMatrix", new Matrix4x4[] { GL.GetGPUProjectionMatrix(xrpass.GetProjMatrix(0), false), GL.GetGPUProjectionMatrix(xrpass.GetProjMatrix(1), false) });

                    Context.ExecuteCommandBuffer(buffer);
                    buffer.Clear();

					ScriptableCullingParameters parameters = xrpass.cullingParams;
                    parameters.SetCullingPlane(4, new Plane(StaticVariables.bluePortalRotation * Vector3.forward, StaticVariables.bluePortalPosition));
                    RenderStateBlock block = new RenderStateBlock(RenderStateMask.Nothing);
                    RenderOpaqueScene(ref Context, RenderCamera, ref parameters, ref block);
                    
                    xrpass.StopSinglePass(buffer);

                    Context.ExecuteCommandBuffer(buffer);
                    buffer.Clear();

                    mirror = true;
                }
            }
			else
            {
                buffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, 0, CubemapFace.Unknown, -1);
                buffer.ClearRenderTarget(RTClearFlags.All, asset.clearColor, 1f, 0u);
                buffer.SetGlobalMatrix("NormalViewMatrix", RenderCamera.worldToCameraMatrix);
                buffer.SetGlobalMatrix("NormalProjMatrix", GL.GetGPUProjectionMatrix(RenderCamera.projectionMatrix, RenderCamera.cameraType == CameraType.SceneView));
                Context.ExecuteCommandBuffer(buffer);
                buffer.Clear();

                RenderCamera.TryGetCullingParameters(out ScriptableCullingParameters parameters);
                parameters.SetCullingPlane(4, new Plane(StaticVariables.bluePortalRotation * Vector3.forward, StaticVariables.bluePortalPosition));
                RenderStateBlock block = new RenderStateBlock(RenderStateMask.Nothing);
                RenderOpaqueScene(ref Context, RenderCamera, ref parameters, ref block);
            }

			if (RenderCamera.stereoEnabled)
            {
                if (mirror)
                {
                    XRSystem.RenderMirrorView(buffer, RenderCamera);
                    Context.ExecuteCommandBuffer(buffer);
                    buffer.Clear();
                }

                XRSystem.EndLayout();
            }

#if UNITY_EDITOR
			if (Handles.ShouldRenderGizmos())
			{
                Context.SetupCameraProperties(RenderCamera);
                Context.DrawGizmos(RenderCamera, GizmoSubset.PreImageEffects);
                Context.DrawGizmos(RenderCamera, GizmoSubset.PostImageEffects);
            }
#endif

			Context.Submit();
        }

        /*
		public void RenderCamera(ref ScriptableRenderContext Context, in Camera RenderCamera)
		{
#if UNITY_EDITOR
			if (RenderCamera.cameraType == CameraType.SceneView)
			{
				ScriptableRenderContext.EmitWorldGeometryForSceneView(RenderCamera);
			}
#endif // UNITY_EDITOR

			Context.SetupCameraProperties(RenderCamera, RenderCamera.stereoEnabled);

			if (RenderCamera.TryGetCullingParameters(RenderCamera.stereoEnabled, out ScriptableCullingParameters parameters) == false)
			{
				return;
			}

			Plane plane = new Plane(StaticVariables.bluePortalRotation * Vector3.back, StaticVariables.bluePortalPosition);
			parameters.SetCullingPlane(4, plane);
			Shader.SetGlobalVector("_ClipPlane", new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance));

			CullingResults results = Context.Cull(ref parameters);

			buffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, 0, CubemapFace.Unknown, -1);

			if (RenderCamera.stereoEnabled)
			{
				buffer.SetSinglePassStereo(SinglePassStereoMode.Instancing);
				Context.StartMultiEye(RenderCamera);
			}
			else
			{
				buffer.SetSinglePassStereo(SinglePassStereoMode.None);
			}

			buffer.ClearRenderTarget(RTClearFlags.All, clear, 1f, 0u);
			buffer.DrawMesh(mesh, Matrix4x4.TRS(StaticVariables.bluePortalPosition, StaticVariables.bluePortalRotation, new Vector3(StaticVariables.bluePortalScale, StaticVariables.bluePortalScale, 1)), material);
			Context.ExecuteCommandBuffer(buffer);
			buffer.Clear();

			SortingSettings sortingSettings = new SortingSettings(RenderCamera);
			sortingSettings.criteria = SortingCriteria.CommonOpaque;

			DrawingSettings drawSettings = new DrawingSettings(new ShaderTagId("SRPDefaultUnlit"), sortingSettings);
			drawSettings.enableDynamicBatching = true;
			drawSettings.enableInstancing = true;
			drawSettings.sortingSettings = sortingSettings;
			drawSettings.perObjectData = 0;
			FilteringSettings filterSettings = new FilteringSettings(RenderQueueRange.opaque);

			RenderStateBlock block = new RenderStateBlock();
			{
				StencilState state = new StencilState
				(
					true, // Enabled
					255, // ReadMask
					255, // WriteMask
					CompareFunction.Equal, // Compare
					StencilOp.Keep, // Pass
					StencilOp.Keep, // Fail
					StencilOp.Keep  // Zfail
				);

				block.mask = RenderStateMask.Stencil;
				block.stencilState = state;
				block.stencilReference = 7;
			}

			Context.DrawRenderers(results, ref drawSettings, ref filterSettings, ref block);
			if (RenderCamera.stereoEnabled)
			{
				Context.StopMultiEye(RenderCamera);
			}

#if UNITY_EDITOR
			if (UnityEditor.Handles.ShouldRenderGizmos())
			{
				Context.DrawGizmos(RenderCamera, GizmoSubset.PreImageEffects);
				Context.DrawGizmos(RenderCamera, GizmoSubset.PostImageEffects);
			}
#endif // UNITY_EDITOR

			if (RenderCamera.stereoEnabled)
			{
				Context.StereoEndRender(RenderCamera);
			}
			Context.Submit();
		}
		*/

        public bool IsCameraProjectionMatrixFlipped()
        {
            if (!SystemInfo.graphicsUVStartsAtTop)
                return false;

            return true;
        }
    }
}