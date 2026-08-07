//Given by kind Aelanna, with like two one-line changes to make it behave with shadows.
//Originally EccentricTransparentComplex
Shader "StagzTransparentComplex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        [Toggle] _MaskIsSRGB ("Mask is sRGB", Integer) = 0
        _Color ("Color 1", Color) = (1,1,1,1)
        _ColorTwo ("Color 2", Color) = (1,1,1,1)
		//Basically my only addition lol. Changes the cutout threshold which it needs in order to behave with shadows properly.
		//0.01 is recommended min for transparent, 0.5 is vanilla cutout behavior +/- some rounding error)		
		_ClipValue ("Clip Threshold", float) = 0.5
    }
    SubShader
    {
        Tags {
            "QUEUE" = "Transparent-100"
            "RenderType" = "Transparent"
        }
        LOD 100
        
        Pass
        {
            ZWrite On
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
			float _ClipValue;

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
                float color_lerp = (1 - color1_i + color2_i) / 2.0;
                float4 combined = lerp(_Color, _ColorTwo, color_lerp);

                float alpha = original.a * combined.a;
				clip(alpha - _ClipValue);
                float3 final = original.rgb * combined.rgb;
                
                return float4(final, alpha);
            }
            ENDHLSL
        }
    }
}
