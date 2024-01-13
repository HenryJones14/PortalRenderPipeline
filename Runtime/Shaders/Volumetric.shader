Shader "PortalRP/Volumetric"
{
    Properties
    {
        _MainTex ("Texture", 3D) = "white" {}
        _quality ("Quality", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Cull Front
        Zwrite Off
        ZTest Always
        Blend One One

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "include.hlsl"

            struct appdata
            {
                float4 position : POSITION;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float4 rayPos : TEXCOORD0;
                float4 rayDir : TEXCOORD1;
            };

            sampler3D _MainTex;
            float _quality;

            v2f vert (appdata v)
            {
                v2f o;
                o.position = mul(unity_ObjectToWorld, v.position);

                o.rayPos = float4(_WorldSpaceCameraPos, 1);
                o.rayDir = o.position - o.rayPos;
                o.rayDir.w = 0;

                o.position = mul(UNITY_MATRIX_VP, o.position);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                Ray ray;
                ray.origin = i.rayPos;
                ray.direction = normalize(i.rayDir);

                Bounds bounds;
                bounds.min = float3(-13, 0, -13);
                bounds.max = float3(13, 26, 13);

                float2 distance_length = RayTraceBounds(ray, bounds);

                int steps = round(distance_length.y * _quality);
                float result = 0;

                for(int i = 0; i < steps; i++)
                {
                    float random = tex3Dlod(_MainTex, float4(ray.direction * distance_length.x, 0)).a * 0.1;
                    result = result + tex3Dlod(_MainTex, float4((ray.origin + ray.direction * random + ray.direction * distance_length.x + ray.direction * (i / _quality)) / 26, 0)).r;
                }

                result = result / (_quality * 13);
                
                return float4(result.rrr, 1);
            }
            ENDCG
        }
    }
}
