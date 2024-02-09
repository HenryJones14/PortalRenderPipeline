using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace PortalRP
{
	public partial class CameraRenderer // All public methods
	{
		private PortalAsset asset;

		private ScriptableRenderContext context;
		private Camera camera;
		private XRPass pass;

		private CommandBuffer commandBuffer;
		private CommandBuffer drawingBuffer;

		public CameraRenderer(PortalAsset Asset)
		{
			asset = Asset;

			XRSystem.Initialize((CreateInfo) => XRPass.CreateDefault(CreateInfo), Shader.Find("Hidden/Universal Render Pipeline/XR/XROcclusionMesh"), Shader.Find("Hidden/Universal Render Pipeline/XR/XRMirrorView"));

			XRSystem.SetDisplayMSAASamples(MSAASamples.None);
			XRSystem.SetRenderScale(1f);

			commandBuffer = new CommandBuffer();
			commandBuffer.name = "PortalRP";

			drawingBuffer = new CommandBuffer();
			drawingBuffer.name = "Draw Objects";
		}

		~CameraRenderer()
		{
			asset = null;

			XRSystem.Dispose();

			commandBuffer.Dispose();
			commandBuffer = null;

			drawingBuffer.Dispose();
			drawingBuffer = null;
		}

		public void RenderAllCameras(ref ScriptableRenderContext RenderContext, List<Camera> RenderCameras)
		{
			XRLayout layout = XRSystem.NewLayout();

			for(int i = 0; i < RenderCameras.Count; i++)
			{
				layout.AddCamera(RenderCameras[i], RenderCameras[i].stereoEnabled);
			}

			foreach((Camera cam, XRPass xr) in layout.GetActivePasses())
			{
				context = RenderContext;
				camera = cam;
				pass = xr;

				RenderSingleCamera();
			}

			XRSystem.EndLayout();
		}

		public void RenderSingleCamera(ref ScriptableRenderContext RenderContext, Camera RenderCamera, XRPass XR)
		{
			context = RenderContext;
			camera = RenderCamera;
			pass = XR;

			RenderSingleCamera();
		}

		private void RenderSingleCamera()
		{
			if(TryGetCullingParameters(out ScriptableCullingParameters parameters))
			{
				// Culling planes:
				// 0 = Left
				// 1 = Right
				// 2 = Down
				// 3 = Up
				// 4 = Near
				// 5 = Far

				Vector3 right = StaticVariables.bluePortalRotation * Vector3.right;
				Vector3 up = StaticVariables.bluePortalRotation * Vector3.up;

				Vector3 bottomLeft = StaticVariables.bluePortalPosition - up - right;
				Vector3 bottomRight = StaticVariables.bluePortalPosition - up + right;
				Vector3 topLeft = StaticVariables.bluePortalPosition + up - right;
				Vector3 topRight = StaticVariables.bluePortalPosition + up + right;

				parameters.SetCullingPlane(0, new Plane(camera.transform.position, topRight, bottomRight));
				parameters.SetCullingPlane(1, new Plane(camera.transform.position, bottomLeft, topLeft));
				parameters.SetCullingPlane(2, new Plane(camera.transform.position, bottomRight, bottomLeft));
				parameters.SetCullingPlane(3, new Plane(camera.transform.position, topLeft, topRight));
				parameters.SetCullingPlane(4, new Plane(StaticVariables.bluePortalRotation * Vector3.back, StaticVariables.bluePortalPosition));
				// Dont need far
			}
			else
			{
				Debug.LogError("Invalid \"ScriptableCullingParameters\"!");
				return;
			}

			StartSinglePass();

			SetAndClearRenderTarget();

			SetGlobalShaderVariables();

			{
				CullingResults results = context.Cull(ref parameters);

				SortingSettings sorting = new SortingSettings(camera);
				sorting.criteria = SortingCriteria.CommonOpaque;

				DrawingSettings drawing = new DrawingSettings(new ShaderTagId("PortalRP"), sorting);
				FilteringSettings filtering = new FilteringSettings(RenderQueueRange.opaque);

				context.DrawRenderers(results, ref drawing, ref filtering);

				drawingBuffer.BeginSample("DrawMesh (StaticVariables)");
				drawingBuffer.DrawMeshInstanced(asset.teapotMesh, 0, asset.teapotMaterial, 0, StaticVariables.instances);

				drawingBuffer.SetGlobalColor("_Color", new Color(0f, 0.5f, 1f));
				drawingBuffer.DrawMesh(asset.portalMesh, StaticVariables.bluePortalMatrix, asset.portalMaterial, 0, 0);
				drawingBuffer.DrawMesh(asset.portalMesh, StaticVariables.bluePortalMatrix, asset.portalMaterial, 0, 1);

				drawingBuffer.SetGlobalColor("_Color", new Color(1f, 0.5f, 0f));
				drawingBuffer.DrawMesh(asset.portalMesh, StaticVariables.orangePortalMatrix, asset.portalMaterial, 0, 0);
				drawingBuffer.DrawMesh(asset.portalMesh, StaticVariables.orangePortalMatrix, asset.portalMaterial, 0, 1);

				for(int i = 0; i < StaticVariables.instances.Length; i++)
				{
					drawingBuffer.DrawMesh(asset.teapotMesh, StaticVariables.instances[i], asset.teapotMaterial, 0, 0);
				}

				drawingBuffer.EndSample("DrawMesh (StaticVariables)");

				context.ExecuteCommandBuffer(drawingBuffer);
				drawingBuffer.Clear();
			}

			StopSinglePass();

			BlitMirrorView();

#if UNITY_EDITOR
			if(UnityEditor.Handles.ShouldRenderGizmos())
			{
				context.SetupCameraProperties(camera);
				context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
				context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
			}
#endif

			context.Submit();
		}

		private void StartSinglePass()
		{
			if(pass.singlePassEnabled)
			{
				if(SystemInfo.supportsMultiview)
				{
					commandBuffer.EnableShaderKeyword("STEREO_MULTIVIEW_ON");
				}
				else
				{
					commandBuffer.EnableShaderKeyword("STEREO_INSTANCING_ON");
					commandBuffer.SetInstanceMultiplier((uint)pass.viewCount);
				}
			}

			context.ExecuteCommandBuffer(commandBuffer);
			commandBuffer.Clear();
		}

		private void SetAndClearRenderTarget()
		{
			if(pass.enabled)
			{
				commandBuffer.SetRenderTarget(pass.renderTarget, 0, CubemapFace.Unknown, -1);
			}
			else
			{

				commandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, 0, CubemapFace.Unknown, -1);
			}

			commandBuffer.ClearRenderTarget(RTClearFlags.All, Color.blue, 1f, 0u);


			context.ExecuteCommandBuffer(commandBuffer);
			commandBuffer.Clear();
		}

		private void SetGlobalShaderVariables()
		{
			if(pass.enabled)
			{
				if(pass.singlePassEnabled)
				{
					Matrix4x4[] view = new Matrix4x4[pass.viewCount];
					Matrix4x4[] proj = new Matrix4x4[pass.viewCount];

					for(int i = 0; i < pass.viewCount; i++)
					{
						view[i] = pass.GetViewMatrix(i);
						proj[i] = GL.GetGPUProjectionMatrix(pass.GetProjMatrix(i), false);
					}

					commandBuffer.SetGlobalMatrixArray("SterioViewMatrix", view);
					commandBuffer.SetGlobalMatrixArray("SterioProjMatrix", proj);
				}
				else
				{
					commandBuffer.SetGlobalMatrix("NormalViewMatrix", pass.GetViewMatrix(0));
					commandBuffer.SetGlobalMatrix("NormalProjMatrix", GL.GetGPUProjectionMatrix(pass.GetProjMatrix(0), false));
				}
			}
			else
			{
				commandBuffer.SetGlobalMatrix("NormalViewMatrix", camera.worldToCameraMatrix);
				commandBuffer.SetGlobalMatrix("NormalProjMatrix", GL.GetGPUProjectionMatrix(camera.projectionMatrix, camera.cameraType == CameraType.SceneView || camera.targetTexture != null));
			}

			context.ExecuteCommandBuffer(commandBuffer);
			commandBuffer.Clear();
		}

		private bool TryGetCullingParameters(out ScriptableCullingParameters CullingParameters)
		{
			if(pass.enabled)
			{
				CullingParameters = pass.cullingParams;
				return true;
			}
			else
			{
				return camera.TryGetCullingParameters(out CullingParameters);
			}
		}

		private void StopSinglePass()
		{
			if(pass.singlePassEnabled)
			{
				if(SystemInfo.supportsMultiview)
				{
					commandBuffer.DisableShaderKeyword("STEREO_MULTIVIEW_ON");
				}
				else
				{
					commandBuffer.DisableShaderKeyword("STEREO_INSTANCING_ON");
					commandBuffer.SetInstanceMultiplier(1);
				}
			}

			context.ExecuteCommandBuffer(commandBuffer);
			commandBuffer.Clear();
		}

		private void BlitMirrorView()
		{
			if(pass.enabled)
			{
				XRSystem.RenderMirrorView(commandBuffer, camera);
				context.ExecuteCommandBuffer(commandBuffer);
				commandBuffer.Clear();
			}
		}
	}
}