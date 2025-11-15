Shader "Skybox/Panoramic_Flip"
{
    Properties
    {
        _MainTex ("Spherical (HDR) Texture", 2D) = "white" {}
        _Exposure ("Exposure", Float) = 1.0
        _Rotation ("Rotation (degrees)", Range(0,360)) = 0
        [Toggle(_FlipY)] _FlipY("Flip Vertically", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Exposure;
            float _Rotation;
            float _FlipY;

            struct v2f {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // Pass a direction vector based on vertex pos (works for skybox cube mesh)
                o.dir = v.vertex.xyz;
                return o;
            }

            // convert view direction to lat-long uv
            float2 dirToLatLongUV(float3 dir)
            {
                dir = normalize(dir);
                float2 uv;
                float phi = atan2(dir.z, dir.x);       // -PI .. PI
                float theta = asin(dir.y);             // -PI/2 .. PI/2
                uv.x = (phi / UNITY_PI + 1.0) * 0.5;   // 0..1
                uv.y = 0.5 - (theta / UNITY_PI);       // 0..1
                return uv;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // apply rotation around Y (horizontal)
                float yaw = radians(_Rotation);
                float3 dir = normalize(i.dir);
                float cy = cos(yaw), sy = sin(yaw);
                float3 dirR = float3(cy * dir.x + sy * dir.z, dir.y, -sy * dir.x + cy * dir.z);

                float2 uv = dirToLatLongUV(dirR);

                // Flip vertically if requested:
                if (_FlipY > 0.5) uv.y = 1.0 - uv.y;

                fixed4 col = tex2D(_MainTex, uv) * _Exposure;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
