float4x4 WorldViewProj;

struct VS_OUT{
	float4 Position : SV_Position;
	float4 PositionCopy : TEXCOORD1;
};

struct PS_IN{
	float4 Coord : TEXCOORD0;
	float4 Position : TEXCOORD1;
};

VS_OUT VS_MAIN(float4 pos : SV_Position)
{
	VS_OUT output = (VS_OUT) 0;

	output.Position = mul(pos, WorldViewProj);
	output.PositionCopy = output.Position;

	return output;
}

float4 PS_MAIN(PS_IN input) : SV_Target0
{
	return float4(input.Position.x / 250, 0, 0, 1);
}

technique{
	pass Pass0 {
		Profile = 9.1;
		VertexShader = VS_MAIN;
		PixelShader = PS_MAIN;
	}
}