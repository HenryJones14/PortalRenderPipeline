// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PortalRP/DefaultUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
            //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

            struct appdata
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID 

                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO

                float4 vertex : SV_POSITION;
                float4 world : TEXCOORD1;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _ClipPlane;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_SETUP_INSTANCE_ID(v);

                o.world = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                //if(dot(float4(o.world.xyz, 1.0), _ClipPlane) > 0.0)
                //{
                    //o.vertex = mul(UNITY_MATRIX_VP, o.world);
                //}
                //else
                //{
                    //o.vertex = float4(0, 0, -1, 1);
                //}

                o.uv = v.uv;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if(dot(float4(i.world.xyz, 1.0), _ClipPlane) > 0.0)
                {
                    return tex2D(_MainTex, i.uv);
                }
                else
                {
                    return fixed4(0, 0, unity_StereoEyeIndex, 1);
                }
            }

            ENDCG
        }
    }
}