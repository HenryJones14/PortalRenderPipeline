using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using static UnityEditor.ShaderData;

namespace PortalRP
{
	internal class PortalRenderer
	{
		// Public variables
		Color clear = Color.clear;

		Mesh mesh;
		Material material;

		// Private variables
		CommandBuffer buffer;
		XRPass xr;

		// Cashed variables


		public PortalRenderer(Color ClearColor, Mesh Mesh, Material Material)
        {
            //XRSystem.dumpDebugInfo = true;

            buffer = new CommandBuffer();
			buffer.name = "PortalRP";

			XRSystem.Initialize(Initialize, Shader.Find("Hidden/Universal Render Pipeline/XR/XROcclusionMesh"), Shader.Find("Hidden/Universal Render Pipeline/XR/XRMirrorView"));

			XRSystem.SetDisplayMSAASamples(MSAASamples.MSAA4x);
			XRSystem.SetRenderScale(1f);

			clear = ClearColor;
			mesh = Mesh;
			material = Material;
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

		public void RenderSingleCamera(ref ScriptableRenderContext Context, in Camera RenderCamera, in XRPass RenderPass)
        {
            CullingResults results;

			if (RenderPass == null)
			{
				Context.SetupCameraProperties(RenderCamera, false, -1);
				buffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
				buffer.ClearRenderTarget(RTClearFlags.All, Color.red, 1f, 0u);

				RenderCamera.TryGetCullingParameters(out ScriptableCullingParameters parameters);
				results = Context.Cull(ref parameters);
			}
			else
            {
                buffer.SetRenderTarget(RenderPass.renderTarget, 0, CubemapFace.Unknown, -1);
				buffer.EnableShaderKeyword("UNITY_STEREO_INSTANCING_ENABLED");

                if (SystemInfo.supportsMultiview)
                {
                    buffer.EnableShaderKeyword("STEREO_MULTIVIEW_ON");
                }
                else
                {
                    buffer.EnableShaderKeyword("STEREO_INSTANCING_ON");
                    buffer.SetInstanceMultiplier(2);
                }

				
                buffer.ClearRenderTarget(RTClearFlags.All, Color.green, 1f, 0u);
                //RenderPass.RenderOcclusionMesh(buffer);

				ScriptableCullingParameters parameters = RenderPass.cullingParams;
				results = Context.Cull(ref parameters);
            }

            //Matrix4x4[] matricies = new Matrix4x4[2] { StaticVariables.bluePortalMatrix, StaticVariables.orangePortalMatrix };
            //buffer.DrawMeshInstanced(mesh, 0, material, 0, matricies);
            buffer.DrawMesh(mesh, StaticVariables.bluePortalMatrix, material);
            buffer.DrawMesh(mesh, StaticVariables.orangePortalMatrix, material);

            Context.ExecuteCommandBuffer(buffer);
			buffer.Clear();


            SortingSettings sortingSettings = new SortingSettings();
			DrawingSettings drawSettings = new DrawingSettings(new ShaderTagId("SRPDefaultUnlit"), sortingSettings);
			FilteringSettings filterSettings = new FilteringSettings(RenderQueueRange.all);
			drawSettings.enableInstancing = true;

			Context.DrawRenderers(results, ref drawSettings, ref filterSettings);


			if (RenderPass != null)
			{
				RenderPass.StopSinglePass(buffer);
                Context.ExecuteCommandBuffer(buffer);
                buffer.Clear();
            }

			Context.Submit();
		}


		public void RenderCamera(ref ScriptableRenderContext Context, Camera RenderCamera)
        {
            //Context.SetupCameraProperties(RenderCamera, RenderCamera.stereoEnabled);
            bool mirror = false;

            if (RenderCamera.stereoEnabled)
            {
                XRLayout layout = XRSystem.NewLayout();
                layout.AddCamera(RenderCamera, RenderCamera);

                foreach ((Camera camera, XRPass xrpass) in layout.GetActivePasses())
                {
                    xrpass.StartSinglePass(buffer);

                    buffer.SetRenderTarget(xrpass.renderTarget, 0, CubemapFace.Unknown, -1);
                    buffer.ClearRenderTarget(RTClearFlags.All, clear, 1f, 0u);

                    buffer.SetGlobalMatrixArray("SterioViewMatrix", new Matrix4x4[] { RenderCamera.worldToCameraMatrix, RenderCamera.worldToCameraMatrix  });
                    buffer.SetGlobalMatrixArray("SterioProjMatrix", new Matrix4x4[] { GL.GetGPUProjectionMatrix(xrpass.GetProjMatrix(0), false), GL.GetGPUProjectionMatrix(xrpass.GetProjMatrix(1), false) });

                    Context.ExecuteCommandBuffer(buffer);
                    buffer.Clear();


                    // Render
                    {
						ScriptableCullingParameters parameters = xrpass.cullingParams;
                        parameters.SetCullingPlane(4, new Plane(Vector3.forward, Vector3.zero));
                        CullingResults results = Context.Cull(ref parameters);

                        SortingSettings sorting = new SortingSettings(RenderCamera);
                        sorting.criteria = SortingCriteria.CommonOpaque;

                        DrawingSettings drawing = new DrawingSettings(new ShaderTagId("SRPDefaultUnlit"), sorting);
                        FilteringSettings filtering = new FilteringSettings(RenderQueueRange.opaque);

                        Context.DrawRenderers(results, ref drawing, ref filtering);

                        buffer.DrawMesh(mesh, StaticVariables.bluePortalMatrix, material);
                        buffer.DrawMesh(mesh, StaticVariables.orangePortalMatrix, material);
                        Context.ExecuteCommandBuffer(buffer);
                        buffer.Clear();
                    }

                    xrpass.StopSinglePass(buffer);

                    Context.ExecuteCommandBuffer(buffer);
                    buffer.Clear();

                    mirror = true;
                }
            }
			else
            {
                buffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, 0, CubemapFace.Unknown, -1);
                buffer.ClearRenderTarget(RTClearFlags.All, clear, 1f, 0u);
                buffer.SetGlobalMatrix("ViewMatrix", RenderCamera.worldToCameraMatrix);
                buffer.SetGlobalMatrix("ProjMatrix", GL.GetGPUProjectionMatrix(RenderCamera.projectionMatrix, RenderCamera.cameraType == CameraType.SceneView));
                Context.ExecuteCommandBuffer(buffer);
                buffer.Clear();

                // Render
                {
                    RenderCamera.TryGetCullingParameters(out ScriptableCullingParameters parameters);
                    parameters.SetCullingPlane(4, new Plane(Vector3.forward, Vector3.zero));
                    CullingResults results = Context.Cull(ref parameters);

                    SortingSettings sorting = new SortingSettings(RenderCamera);
                    sorting.criteria = SortingCriteria.CommonOpaque;

                    DrawingSettings drawing = new DrawingSettings(new ShaderTagId("SRPDefaultUnlit"), sorting);
                    FilteringSettings filtering = new FilteringSettings(RenderQueueRange.opaque);

                    Context.DrawRenderers(results, ref drawing, ref filtering);

                    buffer.DrawMesh(mesh, StaticVariables.bluePortalMatrix, material);
                    buffer.DrawMesh(mesh, StaticVariables.orangePortalMatrix, material);
                    Context.ExecuteCommandBuffer(buffer);
                    buffer.Clear();
                }
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