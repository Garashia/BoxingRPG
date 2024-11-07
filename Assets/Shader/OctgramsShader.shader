Shader "Custom/OctgramsShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _TimeScale("Time Scale", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float _TimeScale;
            float2 _Resolution;

            float gTime = 0.0;
            const float REPEAT = 5.0;

            // Rotation matrix
            float2x2 rot(float a) {
                float c = cos(a), s = sin(a);
                return float2x2(c, s, -s, c);
            }

            float sdBox(float3 p, float3 b) {
                float3 q = abs(p) - b;
                return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
            }

            float box(float3 pos, float scale) {
                pos *= scale;
                float base = sdBox(pos, float3(0.4, 0.4, 0.1)) / 1.5;
                pos.xy *= 5.0;
                pos.y -= 3.5;
                pos.xy = mul(rot(0.75), pos.xy);
                return -base;
            }

            float box_set(float3 pos, float iTime) {
                float3 pos_origin = pos;
                pos.y += sin(gTime * 0.4) * 2.5;
                pos.xy = mul(rot(0.8), pos.xy);
                float box1 = box(pos, 2.0 - abs(sin(gTime * 0.4)) * 1.5);

                pos = pos_origin;
                pos.y -= sin(gTime * 0.4) * 2.5;
                pos.xy = mul(rot(0.8), pos.xy);
                float box2 = box(pos, 2.0 - abs(sin(gTime * 0.4)) * 1.5);

                pos = pos_origin;
                pos.x += sin(gTime * 0.4) * 2.5;
                pos.xy = mul(rot(0.8), pos.xy);
                float box3 = box(pos, 2.0 - abs(sin(gTime * 0.4)) * 1.5);

                pos = pos_origin;
                pos.x -= sin(gTime * 0.4) * 2.5;
                pos.xy = mul(rot(0.8), pos.xy);
                float box4 = box(pos, 2.0 - abs(sin(gTime * 0.4)) * 1.5);

                pos = pos_origin;
                pos.xy = mul(rot(0.8), pos.xy);
                float box5 = box(pos, 0.5) * 6.0;

                pos = pos_origin;
                float box6 = box(pos, 0.5) * 6.0;

                return max(max(max(max(max(box1, box2), box3), box4), box5), box6);
            }

            float map(float3 pos, float iTime) {
                return box_set(pos, iTime);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float iTime = _Time.y * _TimeScale;
                float2 fragCoord = i.uv * _Resolution;
                float2 p = (fragCoord * 2.0 - _Resolution) / min(_Resolution.x, _Resolution.y);
                float3 ro = float3(0.0, -0.2, iTime * 4.0);
                float3 ray = normalize(float3(p, 1.5));
                ray.xy = mul(rot(sin(iTime * 0.03) * 5.0), ray.xy);
                ray.yz = mul(rot(sin(iTime * 0.05) * 0.2), ray.yz);

                float t = 0.1;
                float3 col = 0.0;
                float ac = 0.0;

                for (int i = 0; i < 99; i++) {
                    float3 pos = ro + ray * t;
                    pos = fmod(pos - 2.0, 4.0) - 2.0;
                    gTime = iTime - float(i) * 0.01;

                    float d = map(pos, iTime);
                    d = max(abs(d), 0.01);
                    ac += exp(-d * 23.0);

                    t += d * 0.55;
                }

                col = ac * 0.02;
                col += float3(0.0, 0.2 * abs(sin(iTime)), 0.5 + sin(iTime) * 0.2);

                return float4(col, 1.0 - t * (0.02 + 0.02 * sin(iTime)));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
