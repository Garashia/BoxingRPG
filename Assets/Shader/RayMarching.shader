Shader "Unlit/motionRaymarching" {
    Properties {
        _Radius("Radius", Range(0.0, 1.0)) = 0.3
        _BlurShadow("BlurShadow", Range(0.0, 50.0)) = 16.0
        _Speed("Speed", Range(0.0, 10.0)) = 2.0
    }
    SubShader {
        Tags{ "Queue" = "Transparent" "LightMode"="ForwardBase"}
        LOD 100

        Pass {
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : POSITION1;
                float4 vertex : SV_POSITION;
            };

            // ���̑傫��
            float _Radius;
            // �u���[�̋���
            float _BlurShadow;
            // ���`�⊮�̑��x
            float _Speed;

            // ���S�Ƃ̋������狅��`��
            float sphere(float3 pos) {
                return length(pos) - _Radius;
            }

            // plane�̕`��
            float plane(float3 pos) {
                // plane�̌X��
                float4 n = float4(0.0, 0.8, 0.0, 1.0);
                return dot(pos, n.xyz) + n.w;
            }

            // �����`�̕`��
            float box(float3 pos) {
                float3 b = _Radius;
                float3 d = abs(pos) - b;
                return length(max(d, 0.0)) + min(max(d.x, max(d.y, d.z)), 0.0);
            }

            // plane�Ƌ��Ƃ̋���
            float getDist(float3 pos) {
                // �����g�Ő��`�⊮
                float time = (sin(_Time.y * _Speed) + 1.0f) * 0.5f;
                float morph = lerp(box(pos), sphere(pos), time);
                return min(plane(pos), morph);
            }

            // �@�����擾
            float3 getNormal(float3 pos) {
                // ��
                float d = 0.001;
                // �@���̌������A�e�ϐ��̕Δ�������v�Z
                return normalize(float3(
                    getDist(pos + float3(d, 0, 0)) - getDist(pos + float3(-d, 0, 0)),
                    getDist(pos + float3(0, d, 0)) - getDist(pos + float3(0, -d, 0)),
                    getDist(pos + float3(0, 0, d)) - getDist(pos + float3(0, 0, -d))
                ));
            }

            // �����Ɍ������ă��C���΂�
            float genShadow(float3 pos, float3 lightDir) {
                float marchingDist = 0.0;
                float c = 0.001;
                float r = 1.0;
                float shadowCoef = 0.5;
                for (float t = 0.0; t < 50.0; t++) {
                    marchingDist = getDist(pos + lightDir * c);
                    // hit������e�𗎂Ƃ�
                    if (marchingDist < 0.001) {
                        return shadowCoef;
                    }
                    // ���e�̌v�Z
                    r = min(r, marchingDist * _BlurShadow / c);
                    c += marchingDist;
                }
                // hit���Ȃ������ꍇ�A���e��`��
                return 1.0 - shadowCoef + r * shadowCoef;
            }

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.pos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float3 pos = i.pos.xyz;
                // ���C�̃x�N�g��
                float3 rayDir = normalize(pos.xyz - _WorldSpaceCameraPos);
                const int StepNum = 30;

                for (int j = 0; j < StepNum; j++) {
                    // ���C��i�߂鋗��
                    float marchingDist = getDist(pos);
                    // �Փˌ��m
                    if (marchingDist < 0.001) {
                        float3 lightDir = _WorldSpaceLightPos0.xyz;
                        float3 normal = getNormal(pos);
                        float3 lightColor = _LightColor0;
                        // ���C���I�u�W�F�N�g�ɂ߂荞�ނ̂�h��
                        float shadow = genShadow(pos + normal * 0.001, lightDir);
                        // ���ςɂ���ĐF��ω�������
                        fixed4 col = fixed4(lightColor * max(dot(normal, lightDir), 0) * max(0.5, shadow), 1.0);
                        // �����̃I�t�Z�b�g
                        col.rgb += fixed3(0.2f, 0.2f, 0.2f);
                        return col;
                    }
                    // ���C��i�߂�
                    pos.xyz += marchingDist * rayDir.xyz;
                }
                // stepNum�񃌃C��i�߂Ă��Փ˂��Ȃ�������s�N�Z���𓧖��ɂ���
                return 0;
            }
            ENDCG
        }
    }
}