Shader "PortalRP/Portal"
{
    Properties
    {
        _StencilRef ("Stencil Referance", Float) = 7
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Stencil
        {
            Ref [_StencilRef]
            Comp Always
            Pass Replace
        }

        ColorMask 0
        ZWrite off

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID

                float4 vertex : POSITION;
            };

            struct v2f
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
                
                float4 vertex : SV_POSITION;
                float4 world : TEXCOORD2;
            };

            float4 _ClipPlane;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.world = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = mul(UNITY_MATRIX_VP, o.world);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                if(dot(float4(i.world.xyz, 1.0), _ClipPlane) > 0.0)
                {
                    return fixed4(0, 1, 0, 1);
                }
                else
                {
                    return fixed4(1, 0, 0, 1);
                }
            }

            ENDCG
        }
    }
}
