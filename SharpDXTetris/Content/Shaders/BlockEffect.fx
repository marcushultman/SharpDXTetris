
Texture2D<float4> Texture : register(t0);
sampler TextureSampler : register(s0);

float4x4 WorldViewProj;

struct VS_IN{
	float4 Position : SV_POSITION;
	float2 TexCoord  : TEXCOORD0;
};

struct PS_IN{
	float4 Position : SV_POSITION;
	float2 TexCoord  : TEXCOORD0;
};

PS_IN VS_MAIN(VS_IN input)
{
	PS_IN output = (PS_IN) 0;

	output.Position = mul(input.Position, WorldViewProj);
	output.TexCoord = input.TexCoord;

	return output;
}

float4 PS_MAIN(PS_IN input) : SV_Target
{
	return Texture.Sample(TextureSampler, input.TexCoord);
}

technique{
	pass Pass0 {
		Profile = 11.0;
		VertexShader = VS_MAIN;
		PixelShader = PS_MAIN;
	}
}