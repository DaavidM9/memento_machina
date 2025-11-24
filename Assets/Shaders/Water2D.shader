Shader "Custom/Water2D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaterColor ("Water Color", Color) = (0.2, 0.5, 0.8, 0.8)
        _WaveOffset ("Wave Offset", Float) = 0
        _WaveFrequency ("Wave Frequency", Float) = 10
        _WaveAmplitude ("Wave Amplitude", Float) = 0.1
        _FillAmount ("Fill Amount", Range(0, 1)) = 1
        
        // Enhanced bubble properties
        _BubbleSpeed ("Bubble Speed", Float) = 1
        _BubbleScale ("Bubble Scale", Float) = 20
        _BubbleDensity ("Bubble Density", Range(0, 1)) = 0.3
        _BubbleSize ("Bubble Size", Range(0.01, 0.2)) = 0.05
        _BubbleColor ("Bubble Color", Color) = (1,1,1,0.5)
        
        // Smoke properties
        _SmokeScale ("Smoke Scale", Float) = 15
        _SmokeSpeed ("Smoke Speed", Float) = 0.5
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
        }

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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _WaterColor;
            float4 _BubbleColor;
            float _WaveOffset;
            float _WaveFrequency;
            float _WaveAmplitude;
            float _FillAmount;
            float _BubbleSpeed;
            float _BubbleScale;
            float _BubbleDensity;
            float _BubbleSize;
            float _SmokeScale;
            float _SmokeSpeed;

            // Improved noise function for better-looking bubbles
            float2 random2(float2 st)
            {
                st = float2(dot(st,float2(127.1,311.7)),
                           dot(st,float2(269.5,183.3)));
                return -1.0 + 2.0 * frac(sin(st) * 43758.5453123);
            }

            float noise(float2 st) 
            {
                float2 i = floor(st);
                float2 f = frac(st);
                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(
                    lerp(dot(random2(i + float2(0.0,0.0)), f - float2(0.0,0.0)),
                         dot(random2(i + float2(1.0,0.0)), f - float2(1.0,0.0)), u.x),
                    lerp(dot(random2(i + float2(0.0,1.0)), f - float2(0.0,1.0)),
                         dot(random2(i + float2(1.0,1.0)), f - float2(1.0,1.0)), u.x),
                    u.y) * 0.5 + 0.5;
            }

            // New function for creating circular bubbles
            float createBubble(float2 uv, float2 center, float size)
            {
                float2 delta = uv - center;
                float dist = length(delta);
                return 1 - smoothstep(0, size, dist);
            }

            v2f vert (appdata v)
            {
                v2f o;
                if (v.uv.y > 0.9) {
                    v.vertex.y += sin(v.uv.x * _WaveFrequency + _WaveOffset) * _WaveAmplitude;
                }
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Base waves
                float wave1 = sin(i.uv.x * _WaveFrequency * 0.5 + _WaveOffset) * 0.5 + 0.5;
                float wave2 = sin(i.uv.x * _WaveFrequency * 0.8 - _WaveOffset * 1.2) * 0.5 + 0.5;
                float waves = (wave1 + wave2) * 0.5;

                // Enhanced bubble system
                float2 bubbleUV = i.uv * _BubbleScale;
                bubbleUV.y += _Time.y * _BubbleSpeed;
                float bubbleNoise = noise(bubbleUV);
                
                // Create multiple bubble layers
                float bubbles = 0;
                float2 bubbleOffset = float2(sin(_Time.y * 0.5), _Time.y * _BubbleSpeed);
                for (int j = 0; j < 3; j++)
                {
                    float2 center = float2(
                        frac(bubbleNoise + bubbleOffset.x + j * 0.37),
                        frac(bubbleNoise + bubbleOffset.y + j * 0.43)
                    );
                    bubbles += createBubble(frac(bubbleUV * 0.1), center, _BubbleSize) * _BubbleDensity;
                }
                bubbles = saturate(bubbles);

                // Surface smoke effect
                float2 smokeUV = i.uv * _SmokeScale;
                smokeUV.x += _Time.y * _SmokeSpeed;
                float smoke = noise(smokeUV) * 0.15;
                smoke *= smoothstep(0.8, 1.0, i.uv.y);

                // Combine effects
                float4 col = _WaterColor;
                float heightGradient = 1 - i.uv.y;
                
                // Add bubbles with bubble color
                col.rgb = lerp(col.rgb, _BubbleColor.rgb, bubbles * _BubbleColor.a);
                col.a *= (waves * 0.2 + 0.8) * (0.8 + heightGradient * 0.2);
                
                // Add smoke
                col.rgb += smoke;

                // Apply fill amount
                if (i.uv.y > _FillAmount) {
                    col.a = 0;
                }
                
                return col;
            }
            ENDCG
        }
    }
}