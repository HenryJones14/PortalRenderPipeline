Shader "PortalRP/PortalMask"
{
    Properties
    {

    }

    SubShader
    {
        Tags { "LightMode"="PortalRP-PortalMask" "RenderType" = "Opaque" }
        
        Pass // Set stencil pass
        {
            Name "SetStencil"

            ZTest LEqual
            ZWrite False

            Offset -1, -1

            Stencil
            {
                Ref 0
                Comp Equal
                Pass IncrSat
                Fail Keep
                ZFail Keep
            }

            HLSLPROGRAM


            // Main functions
            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram


            // Includes
            #include "Packages/com.gameboxinteractive.portalrp/ShaderLibrary/UnityEngineDefines.hlsl"
            #include "Packages/com.gameboxinteractive.portalrp/ShaderLibrary/UnityEngineInput.hlsl"



            // Input/Output structures
            struct VertexInput
            {
                float4 pos : POSITION;

                uint instanceID : SV_InstanceID;
            };

            struct FragmentInput
            {
                float4 pos : SV_POSITION;
                
                //uint instanceID : SV_InstanceID;
                uint stereoEyeID : SV_RenderTargetArrayIndex;
            };



            // Uniform variables
            float4 _Color;


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

                return output;
            }

            float4 FragmentProgram () : SV_TARGET
            {
                return _Color;
            }



            ENDHLSL

        } // Set stencil pass

        Pass // Clear depth pass
        {
            Name "ClearDepth"

            ZTest Always
            ZWrite True
            ColorMask 0

            Stencil
            {
                Ref 1
                Comp Equal
                Pass Keep
                Fail Keep
                ZFail Keep
            }

            HLSLPROGRAM


            // Main functions
            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram


            // Includes
            #include "Packages/com.gameboxinteractive.portalrp/ShaderLibrary/UnityEngineDefines.hlsl"
            #include "Packages/com.gameboxinteractive.portalrp/ShaderLibrary/UnityEngineInput.hlsl"



            // Input/Output structures
            struct VertexInput
            {
                float4 pos : POSITION;

                uint instanceID : SV_InstanceID;
            };

            struct FragmentInput
            {
                float4 pos : SV_POSITION;
                
                //uint instanceID : SV_InstanceID;
                uint stereoEyeID : SV_RenderTargetArrayIndex;
            };



            // Uniform variables



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

                #if defined(UNITY_REVERSED_Z)
                    output.pos.z = 1.0e-9f;
                #else
                    output.pos.z = output.pos.w - 1.0e-9f;
                #endif


                return output;
            }

            void FragmentProgram () { }



            ENDHLSL

        } // Clear depth pass
    }
}