Shader "Custom/BeatReactiveShader"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
        _BeatIntensity ("Beat Intensity", Float) = 0
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
            fixed4 _Color;
            float _BeatIntensity;

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return _Color * (1 + _BeatIntensity);
            }
            ENDCG
        }
    }
}