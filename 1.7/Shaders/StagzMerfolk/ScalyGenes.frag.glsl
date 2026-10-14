//modified CutoutComplex
#version 450

layout(location = 0) in vec4 vColor;
layout(location = 1) in vec2 vUv;

layout(location = 0) out vec4 outColor;

layout(set = 2, binding = 0) uniform MaterialUniforms
{
    vec4 _Color;      // @color @default = vec4(1.0, 1.0, 1.0, 1.0)
    vec4 _MainTex_ST; // @default = vec4(0.0, 0.0, 1.0, 1.0)
    vec4 _ColorTwo;   // @color @default = vec4(1.0, 1.0, 1.0, 1.0)
} uMaterial;

layout(set = 2, binding = 1) uniform sampler2D _MainTex;
layout(set = 2, binding = 2) uniform sampler2D _MaskTex; // @default = black

void main()
{
    vec2 uv = vUv * uMaterial._MainTex_ST.zw + uMaterial._MainTex_ST.xy;
    vec4 f = texture(_MainTex, uv);
    vec4 colPower = texture(_MaskTex, uv);
    
    f *= mix(uMaterial._Color, uMaterial._ColorTwo, (1 - colPower.r + colPower.g) / 2);

    if (f.a < 0.01) discard;
    outColor = f;
}