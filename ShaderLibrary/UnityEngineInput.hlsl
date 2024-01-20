#pragma once

// Random shit
float4x4 unity_ObjectToWorld;
float4x4 unity_WorldToObject;

#define MATRIX_M unity_ObjectToWorld
#define MATRIX_I_M unity_WorldToObject

#if defined(USING_STEREO_MATRICES)

	float4x4 SterioViewMatrix[2];
	float4x4 SterioProjMatrix[2];

	#define MATRIX_V SterioViewMatrix[unity_StereoEyeIndex]
	#define MATRIX_P SterioProjMatrix[unity_StereoEyeIndex]

#else

	float4x4 NormalViewMatrix;
	float4x4 NormalProjMatrix;

	#define MATRIX_V NormalViewMatrix
	#define MATRIX_P NormalProjMatrix

#endif