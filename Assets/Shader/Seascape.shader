Shader "Custom/Seascape"
{
    Properties
    {
        _TimeSpeed ("Sea Speed", Float) = 0.8
        _Choppiness ("Choppiness", Float) = 4.0
        _SeaHeight ("Sea Height", Float) = 0.6
        _SeaFreq ("Sea Frequency", Float) = 0.16
        _SeaBase ("Sea Base Color", Color) = (0.1, 0.19, 0.22, 1.0)
        _SeaWaterColor ("Sea Water Color", Color) = (0.8, 0.9, 0.6, 1.0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _TimeSpeed;
            float _Choppiness;
            float _SeaHeight;
            float _SeaFreq;
            float4 _SeaBase;
            float4 _SeaWaterColor;

            const float PI = 3.141592;
            const int ITER_GEOMETRY = 3;
            const int ITER_FRAGMENT = 5;

            float hash(float2 p)
            {
                float h = dot(p, float2(127.1, 311.7));
                return frac(sin(h) * 43758.5453123);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);

                float a = hash(i + float2(0.0, 0.0));
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));

                return (2.0 * lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y)) - 1.0;
            }

            float sea_octave(float2 uv, float choppy)
            {
                uv += noise(uv);
                float2 wv = 1.0 - abs(sin(uv));
                float2 swv = abs(cos(uv));
                wv = lerp(wv, swv, wv);
                return pow(1.0 - pow(wv.x * wv.y, 0.65), choppy);
            }

            float map(float3 p)
            {
                float freq = _SeaFreq;
                float amp = _SeaHeight;
                float choppy = _Choppiness;
                float2 uv = p.xz;
                float h = 0.0;

                for (int i = 0; i < ITER_GEOMETRY; i++)
                {
                    float d = sea_octave((uv + _Time.y * _TimeSpeed) * freq, choppy);
                    d += sea_octave((uv - _Time.y * _TimeSpeed) * freq, choppy);
                    h += d * amp;
                    uv = mul(uv, float2x2(1.6, 1.2, -1.2, 1.6));
                    freq *= 2.0;
                    amp *= 0.2;
                    choppy = lerp(choppy, 1.0, 0.2);
                }

                return p.y - h;
            }

            float3 getSkyColor(float3 e)
            {
                e.y = max(e.y, 0.0);
                return lerp(float3(0.6, 1.0, 0.6), float3(0.1, 0.19, 0.22), pow(1.0 - e.y, 2.0));
            }

            float3 getSeaColor(float3 p, float3 n, float3 l, float3 eye, float3 dist)
            {
                float fresnel = pow(1.0 - dot(n, -eye), 3.0) * 0.65;
                float3 reflected = getSkyColor(reflect(eye, n));
                float3 refracted = _SeaBase.rgb + dot(n, l) * _SeaWaterColor.rgb * 0.12;
                float3 color = lerp(refracted, reflected, fresnel);
                return color;
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float heightMapTracing(float3 ori, float3 dir, out float3 p)
            {
                float t = 0.0;
                p = ori + dir * t;
                float dist = map(p);
                while (dist > 0.001 && t < 100.0)
                {
                    t += dist;
                    p = ori + dir * t;
                    dist = map(p);
                }
                return t;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv * 2.0 - 1.0;
                float3 ori = float3(0.0, 3.5, 5.0);
                float3 dir = normalize(float3(uv.xy, -2.0));
                float3 lightDir = normalize(float3(0.0, 1.0, 0.8));

                float3 p;
                float t = heightMapTracing(ori, dir, p);

                if (t >= 100.0)
                {
                    return float4(getSkyColor(dir), 1.0);
                }

                // Unity‚Å‚Ì‰ð‘œ“x‚ÉŠî‚Ã‚­EPSILON_NRM
                float EPSILON_NRM = 0.1 / _ScreenParams.x;

                float3 dist = p - ori;
                float3 n = normalize(float3(map(p + float3(EPSILON_NRM, 0, 0)) - map(p), map(p + float3(0, 0, EPSILON_NRM)) - map(p), EPSILON_NRM));
                float3 color = getSeaColor(p, n, lightDir, dir, dist);

                return float4(color, 1.0);
            }
            ENDCG
        }
    }
}
