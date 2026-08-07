//Original code given by Aelanna. Kept as reference and not compiled in the bundle
Shader "Eccentric/TransparentComplex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        [Toggle] _MaskIsSRGB ("Mask is sRGB", Integer) = 0
        _Color ("Color 1", Color) = (1,1,1,1)
        _ColorTwo ("Color 2", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags {
            "QUEUE" = "Transparent-100"
            "RenderType"="Transparent"
        }
        LOD 100
        
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct vertexInput
            {
                float4 vertex : POSITION;
                float2 texture_pos : TEXCOORD0;
            };

            struct vertexOutput
            {
                float2 uv : TEXCOORD0;
                float4 clip_pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;
            float _MaskIsSRGB;
            float4 _Color;
            float4 _ColorTwo;

            vertexOutput vert (vertexInput v)
            {
                vertexOutput output;
                output.clip_pos = UnityObjectToClipPos(v.vertex);
                output.uv = TRANSFORM_TEX(v.texture_pos, _MainTex);
                return output;
            }

            fixed4 frag (vertexOutput input) : SV_Target
            {
                fixed4 original = tex2D(_MainTex, input.uv);
                fixed4 mask = tex2D(_MaskTex, input.uv);

                float color1_i = lerp(mask.r, LinearToGammaSpace(mask.r), step(0.5, _MaskIsSRGB));
                float color2_i = lerp(mask.g, LinearToGammaSpace(mask.g), step(0.5, _MaskIsSRGB));
                float total_i = color1_i + color2_i;
                float average_i = total_i / 2.0;
                float color_lerp = (1 - color1_i + color2_i) / 2.0;
                float4 combined = lerp(_Color, _ColorTwo, color_lerp);

                float4 blended = lerp(float4(1,1,1,1), combined, average_i);
                
                float alpha = original.a * blended.a;
                float3 final = original.rgb * combined.rgb;
                
                return float4(final, alpha);
            }
            ENDHLSL
        }
    }
}
