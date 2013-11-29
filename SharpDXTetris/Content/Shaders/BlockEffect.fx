Texture2D<float4> TestTexture : register(t0);
sampler TestSampler : register(s0);

float4x4 WorldViewProj;

struct VS_IN{
	float4 Position : SV_Position;
	float2 TexCoord  : TEXCOORD0;
};

struct VS_OUT{
	float4 Position : SV_Position;
	float2 TexCoord  : TEXCOORD0;
};

struct PS_IN{
	float2 TexCoord  : TEXCOORD0;
};

VS_OUT VS_MAIN(VS_IN input)
{
	VS_OUT output = (VS_OUT) 0;

	output.Position = mul(input.Position, WorldViewProj);
	//output.PositionCopy = input.Position;

	output.TexCoord = input.TexCoord;

	return output;
}

float4 PS_MAIN(PS_IN input) : SV_Target0
{
	return TestTexture.Sample(TestSampler, input.TexCoord);
}

technique{
	pass Pass0 {
		Profile = 11.0;
		VertexShader = VS_MAIN;
		PixelShader = PS_MAIN;
	}
}