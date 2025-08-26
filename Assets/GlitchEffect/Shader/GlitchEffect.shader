Shader "Hidden/GlitchEffect"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _Intensity("Intensity", Range(0, 1)) = 0.5
        _Distortion("Distortion", Float) = 10
        _EffectType("Effect Type", Int) = 0
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Intensity;
            float _Distortion;
            int _EffectType;

            float rand(float2 seed)
            {
                return frac(sin(dot(seed.xy, float2(12.9898,78.233))) * 43758.5453);
            }

            float noise(float2 uv)
            {
                return rand(uv + floor(_Time.y * 1000));
            }

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = float2(i.uv.x, 1.0 - i.uv.y);
                fixed4 col = tex2D(_MainTex, uv);

                 switch(_EffectType)
                {
                    case 0: 
                    {
                        float scanline = rand(float2(0, floor(uv.y * _Distortion * 10.0 + _Time.y * 10.0)));
                        uv.x += (scanline - 0.5) * _Intensity * 0.2;
                        float2 offset = float2(_Intensity * 0.01, 0);
                        col.r = tex2D(_MainTex, uv + offset).r;
                        col.g = tex2D(_MainTex, uv).g;
                        col.b = tex2D(_MainTex, uv - offset).b;
                        break;
                    }

                     case 1:
                    {
                         float2 block = floor(uv * _Distortion * 2 + float2(_Time.y * 3, 0));
                    uv.x += (rand(block) - 0.5) * _Intensity * 0.2;
                    uv.y += (rand(block + 0.5) - 0.5) * _Intensity * 0.2;

                    float2 offset = float2(_Intensity * 0.1 * sin(_Time.y * 10), 0);
                    col.r = tex2D(_MainTex, uv + offset).r;
                    col.g = tex2D(_MainTex, uv).g;
                    col.b = tex2D(_MainTex, uv - offset).b;

                    float scanline = sin(uv.y * 800 + _Time.y * 500) * 0.2;
                    col.rgb -= scanline * _Intensity;
                        break;
                    }
                    
                    case 2: 
                    {
                       
                    uv.y += sin(_Time.y * _Distortion * 0.5) * _Intensity * 0.05;
                    
                   
                    float drift = sin(_Time.y * 2) * _Intensity * 0.1;
                    col.r = tex2D(_MainTex, uv + float2(drift, 0)).r;
                    col.b = tex2D(_MainTex, uv - float2(drift, 0)).b;

                    
                    float noiseVal = rand(uv * 100 + floor(_Time.y * 30));
                    col.rgb += (noiseVal - 0.5) * _Intensity * 0.5;
                        break;
                    }
                    
                    case 3: 
                    {
                       
                    float2 noiseUV = uv * _Distortion * 0.5 + _Time.y;
                    float2 displacement = float2(rand(noiseUV), rand(noiseUV + 0.5)) - 0.5;
                    uv += displacement * _Intensity * 0.2;

                    col.rgb = tex2D(_MainTex, uv).rgb;
                    float noiseVal = rand(uv * 1000 + floor(_Time.y * 60));
                    col.rgb += (noiseVal - 0.5) * _Intensity * 1.2;

                    col.rgb *= 1.0 + sin(_Time.y * 30) * _Intensity * 0.3;
                        break;
                    }

                   case 4: 
{
    float blockSize = 50 + (1 - _Intensity) * 100;
    float2 block = floor(uv * blockSize + _Time.y * 2.0); 
    float2 blockOffset = float2(rand(block), rand(block + 0.5)) * 0.1 * _Intensity;
    uv += blockOffset * sin(_Time.y * 5.0); 

    float2 rOffset = float2(
        rand(block + float2(_Time.y, 0.0)), 
        rand(block + float2(0.0, _Time.y))
    ) * 0.05 * _Intensity;
    
    float2 bOffset = float2(
        rand(block + float2(_Time.y, 1.0)), 
        rand(block + float2(1.0, _Time.y))
    ) * 0.05 * _Intensity;
    
    col.r = tex2D(_MainTex, uv + rOffset).r;
    col.g = tex2D(_MainTex, uv).g;
    col.b = tex2D(_MainTex, uv - bOffset).b;

    float timeSeed = floor(_Time.y * 30.0); 
    if(rand(block * timeSeed) > 0.92) {
        float2 distortedUV = uv * (0.8 + sin(_Time.y) * 0.1) + 0.1;
        col = tex2D(_MainTex, distortedUV);
    }
    
    col.r = tex2D(_MainTex, uv + float2(_Intensity * 0.02 * sin(_Time.y * 2.0), 0)).r;
    col.b = tex2D(_MainTex, uv - float2(_Intensity * 0.02 * cos(_Time.y * 2.0), 0)).b;
    break;
}


                    case 5: 
                    {
                        float lineIntensity = sin(uv.y * 300 + _Time.y * 10) * 2;
                        lineIntensity = pow(abs(lineIntensity), 50) * 10 * _Intensity;
                        uv.x += lineIntensity * (rand(float2(uv.y, _Time.y)) - 0.5) * 0.1;
                        
                        float flash = step(0.99, rand(float2(_Time.y, 0)));
                        col.rgb = lerp(col.rgb, float3(1,0,0), flash * _Intensity);
                        
                        float stripe = step(0.98, rand(float2(uv.x * 100, _Time.y)));
                        col.rgb += stripe * float3(0, 0.5, 1) * _Intensity;
                        break;
                    }

                    case 6: 
                    {
                        float pixelSize = lerp(512, 4, _Intensity);
                        float2 pixelUV = floor(uv * pixelSize) / pixelSize;
                        pixelUV += 0.5 / pixelSize;
                        
                        fixed4 pixelCol = tex2D(_MainTex, pixelUV);
                        pixelCol.rgb = floor(pixelCol.rgb * 8) / 8;
                        
                        float scanline = sin(uv.y * 800 + _Time.y * 10) * 0.2;
                        pixelCol.rgb -= scanline * _Intensity;
                        
                        col.rgb = lerp(col.rgb, pixelCol.rgb, _Intensity);
                        break;
                    }

                    case 7: 
                    {
                        float trackLines = step(0.95, rand(float2(0, uv.y * 30 + _Time.y)));
                        col.rgb *= 1.0 - trackLines * 0.5 * _Intensity;
                        
                        uv.x += sin(uv.y * 50 + _Time.y * 5) * 0.01 * _Intensity;
                        
                        float smear = rand(float2(uv.yy + _Time.y)) * 0.1 * _Intensity;
                        col.r = tex2D(_MainTex, uv + float2(smear * 0.5, 0)).r;
                        col.b = tex2D(_MainTex, uv - float2(smear * 0.3, 0)).b;
                        
                        float noisePattern = rand(uv * 100 + floor(_Time.y * 30));
                        col.rgb += (noisePattern - 0.5) * 0.2 * _Intensity;
                        break;
                    }

                    case 8: 
                    {
                        float tear = step(0.99, rand(float2(0, _Time.y))) * _Intensity;
                        float tearOffset = (rand(float2(uv.y, _Time.y)) - 0.5) * 0.3;
                        uv.x += tearOffset * tear;
                        
                        float edge = 1.0 - smoothstep(0.0, 0.005, abs(frac(uv.y * 50 + _Time.y) - 0.5));
                        col.rgb = lerp(col.rgb, float3(0,0,0), edge * _Intensity);
                        break;
                    }
                    case 9:
{
    float2 center = float2(0.5, 0.5);
    float2 dir = normalize(uv - center);
    float dist = length(uv - center);
    
    float wave1 = sin(dist * _Distortion * 30 - _Time.y * 6) * 0.15;
    float wave2 = cos(dist * _Distortion * 25 - _Time.y * 4) * 0.1;
    float wave3 = sin((uv.x + uv.y) * _Distortion * 50 + _Time.y * 8) * 0.05;
    
    uv += dir * (wave1 + wave2 + wave3) * _Intensity;
    
    float2 rgbShift = dir * _Intensity * 0.1 * sin(_Time.y * 5);
    col.r = tex2D(_MainTex, uv + rgbShift).r;
    col.g = tex2D(_MainTex, uv).g;
    col.b = tex2D(_MainTex, uv - rgbShift).b;
    
    col.rgb *= 1.0 + 0.3 * sin(dist * 40 - _Time.y * 10);
    break;
}


case 10: 
{
    float luminance = dot(col.rgb, float3(0.3, 0.6, 0.1));
    float3 thermal;
    thermal.r = smoothstep(0.2, 0.8, luminance) * 2.0;
    thermal.g = smoothstep(0.3, 0.6, luminance);
    thermal.b = smoothstep(0.8, 0.9, luminance) * 0.5;
    
    float scanNoise = rand(uv * 100 + floor(_Time.y * 30)) * 0.3;
    col.rgb = lerp(col.rgb, thermal, _Intensity) + scanNoise * _Intensity;
    break;
}

case 11: 
{
    float scanLine = sin(uv.y * 800 + _Time.y * 10) * 0.3;
    
    float2 offset = float2(sin(_Time.y), cos(_Time.y)) * _Intensity * 0.02;
    col.r = tex2D(_MainTex, uv + offset).r;
    col.g = tex2D(_MainTex, uv).g;
    col.b = tex2D(_MainTex, uv - offset).b;
    
    float flicker = rand(float2(_Time.y, 0)) * 0.3;
    col.rgb = lerp(col.rgb, col.rgb * float3(0,1,1), _Intensity);
    col.rgb *= 1.0 - flicker * _Intensity;
    
    col.rgb += smoothstep(0.5, 1.0, col.r) * _Intensity * 0.5;
    break;
}

case 12: 
{
    float2 block = floor(uv * 20);
    uv.x += (rand(block + floor(_Time.y * 2)) - 0.5) * _Intensity * 0.1;
    uv.y += (rand(block + floor(_Time.y * 3)) - 0.5) * _Intensity * 0.1;
    
    float2 shift = float2(_Intensity * 0.02, 0);
    col.r = tex2D(_MainTex, uv + shift).r;
    col.g = tex2D(_MainTex, uv).g;
    col.b = tex2D(_MainTex, uv - shift).b;
    
    if(rand(floor(_Time.y * 10)) > 0.98) {
        col.rgb = 1.0 - col.rgb;
    }
    break;
}
                } 

                return col;
            }
            ENDCG
        }
    }
}