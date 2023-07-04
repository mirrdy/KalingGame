Shader "Custom/CircularGaugeShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _FillOrigin("Fill Origin", Range(0, 6.283185)) = 0
    }

        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha
                Cull Off
                ZWrite Off
                ZTest Always

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
                    float4 vertex : SV_POSITION;
                };

                float _FillOrigin; // Radian value for fill origin

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 p = i.uv - 0.5;
                    float angle = atan2(p.y, p.x);
                    float normalizedAngle = (angle - _FillOrigin) / (2 * 3.14159);
                    normalizedAngle = frac(normalizedAngle + 1.0);

                    // Apply fill based on angle
                    fixed4 color = (normalizedAngle >= 0.0 && normalizedAngle <= 0.333) ? fixed4(1, 1, 1, 1) :
                                   (normalizedAngle > 0.333 && normalizedAngle <= 0.666) ? fixed4(1, 1, 1, 1) :
                                   fixed4(1, 1, 1, 0);

                    return color;
                }
                ENDCG
            }
        }
}