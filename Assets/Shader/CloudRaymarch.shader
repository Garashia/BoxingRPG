Shader "Custom/CloudRaymarch"
{
    Properties
    {
        _CloudColor ("Cloud Color", Color) = (1, 1, 1, 1)
        _LightDirection ("Light Direction", Vector) = (0, 1, 0)
        _Density ("Cloud Density", Range(0.1, 1.0)) = 0.5
        _StepSize ("Step Size", Range(0.001, 0.1)) = 0.01
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _CloudColor;
            float3 _LightDirection;
            float _Density;
            float _StepSize;

            // ÉpÅ[ÉäÉìÉmÉCÉYä÷êî (ä»à’î≈)
            float hash(float n)
            {
                return frac(sin(n) * 43758.5453);
            }

            float noise(float3 p)
            {
                float3 i = floor(p);
                float3 f = frac(p);

                f = f*f*(3.0-2.0*f);

                float n = dot(i, float3(1.0, 57.0, 113.0));

return lerp(
    lerp(
        lerp(
            hash(n + dot(float3(0.0, 0.0, 0.0), float3(1.0, 1.0, 1.0))),  // èCê≥ÅFà¯êîÇÃÉxÉNÉgÉãóvëfÇ™ïsë´ÇµÇƒÇ¢ÇΩÇ©Ç‡ÇµÇÍÇ‹ÇπÇÒ
            hash(n + dot(float3(1.0, 0.0, 0.0), float3(1.0, 1.0, 1.0))),  // èCê≥
            f.x
        ),
        lerp(
            hash(n + dot(float3(0.0, 1.0, 0.0), float3(1.0, 1.0, 1.0))),  // èCê≥
            hash(n + dot(float3(1.0, 1.0, 0.0), float3(1.0, 1.0, 1.0))),  // èCê≥
            f.x
        ),
        f.y
    ),
    lerp(
        lerp(
            hash(n + dot(float3(0.0, 0.0, 1.0), float3(1.0, 1.0, 1.0))),  // èCê≥
            hash(n + dot(float3(1.0, 0.0, 1.0), float3(1.0, 1.0, 1.0))),  // èCê≥
            f.x
        ),
        lerp(
            hash(n + dot(float3(0.0, 1.0, 1.0), float3(1.0, 1.0, 1.0))),  // èCê≥
            hash(n + dot(float3(1.0, 1.0, 1.0), float3(1.0, 1.0, 1.0))),  // èCê≥
            f.x
        ),
        f.y
    ),
    f.z
);            }

            // â_ÇÃå`èÛÇï\Ç∑ä÷êî
            float cloudDensity(float3 p)
            {
                float n = 0.0;
                float scale = 1.0;
                float weight = 0.5;

                for (int i = 0; i < 5; i++)
                {
                    n += noise(p * scale) * weight;
                    scale *= 2.0;
                    weight *= 0.5;
                }

                return n;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 rayDir = normalize(i.worldPos - _WorldSpaceCameraPos.xyz);
                float3 rayPos = _WorldSpaceCameraPos.xyz;

                float accumulatedDensity = 0.0;
                float3 accumulatedColor = float3(0.0, 0.0, 0.0);
                float totalSteps = 0.0;

                for (int j = 0; j < 100; j++)
                {
                    float3 samplePos = rayPos + rayDir * _StepSize * j;
                    float density = cloudDensity(samplePos) * _Density;

                    accumulatedDensity += density;
                    totalSteps += 1.0;

                    if (accumulatedDensity > 1.0)
                        break;
                }

                float alpha = accumulatedDensity / totalSteps;
                float3 cloudColor = _CloudColor.rgb * alpha;

                // ÉâÉCÉeÉBÉìÉOåvéZ
                float lightFactor = max(dot(normalize(_LightDirection), rayDir), 0.0);
                cloudColor *= lightFactor;

                return float4(cloudColor, alpha);
            }
            ENDCG
        }
    }
}
