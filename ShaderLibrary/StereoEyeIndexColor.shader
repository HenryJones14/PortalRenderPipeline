Shader "PortalRP/StereoEyeIndexColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "LightMode"="PortalRP" "RenderType" = "Opaque" }

        Pass
        {
            HLSLPROGRAM



            // Main functions
            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram


            // Compile options
            #pragma multi_compile_instancing


            // Includes
            #include "Packages/com.gameboxinteractive.portalrp/ShaderLibrary/UnityEngineDefines.hlsl"
            #include "Packages/com.gameboxinteractive.portalrp/ShaderLibrary/UnityEngineInput.hlsl"



            // Input/Output structures
            struct VertexInput
            {
                float4 pos : POSITION;
                float4 uvs : TEXCOORD;

                uint instanceID : SV_InstanceID;
            };

            struct FragmentInput
            {
                float4 pos : SV_POSITION;
                float4 uvs : TEXCOORD;
                
                uint instanceID : SV_InstanceID;
                uint stereoEyeID : SV_RenderTargetArrayIndex;
            };



            // Uniform variables
            sampler2D _MainTex;



            // Vertex/Fragment programs
            FragmentInput VertexProgram (VertexInput input)
            {
                FragmentInput output;
                
                unity_StereoEyeIndex = input.instanceID & 0x01;
                unity_InstanceID = unity_BaseInstanceID + (input.instanceID >> 1);

                output.stereoEyeID = unity_StereoEyeIndex;
                
                output.pos = mul(MATRIX_M, input.pos);
                output.pos = mul(MATRIX_V, output.pos);
                output.pos = mul(MATRIX_P, output.pos);

                output.uvs = input.uvs;


                return output;
            }

            float4 FragmentProgram (FragmentInput input) : SV_Target
            {
                float4 col = tex2D(_MainTex, input.uvs.xy);
                
                col.r = col.r * input.stereoEyeID;
                col.g = col.g * (1 - input.stereoEyeID);
                col.b = 0;

                return col;
            }



            ENDHLSL

        } // Pass
    }
}