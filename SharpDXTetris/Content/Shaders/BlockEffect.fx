float4 PS_MAIN(float2 coord : TEXCOORD) : SV_TARGET
{
	return float4(coord.r, coord.g, 0, 1);
}

technique{
	pass Pass0 {
		Profile = 11.0;
		PixelShader = PS_MAIN;
	}
}