
#define PURPLE	float3(0.719f, 0, 1)
#define BLUE	float3(0, 0, 1)
#define GREEN	float3(0, 1, 0)
#define WHITE	float3(1, 1, 1)

void holeColor_float(in float3 worldPos, in float2 purplePointPos, out float4 color)
{
	float3 opos = mul(unity_ObjectToWorld, float4(worldPos, 1)).xyz;
	float3 ppos = mul(unity_ObjectToWorld, float4(purplePointPos, 0, 1)).xyz;
	float dist = distance(worldPos, ppos);
	const float purpleLimit = 0.08f;
	const float blueLimit = 0.12f;
	const float greenLimit = 0.4f;

	if (dist < purpleLimit)
	{
		float purpleToBlue = smoothstep(0.0f, blueLimit, dist);
		color.rgb = lerp(PURPLE, BLUE, purpleToBlue);
	}
	else if (dist < blueLimit)
	{
		float purpleToBlue = smoothstep(0.0f, blueLimit, dist);
		float blueToGreen = smoothstep(purpleLimit, blueLimit, dist);
		color.rgb = lerp(PURPLE, BLUE, purpleToBlue);
		color.rgb = lerp(color, GREEN, blueToGreen);
	}
	else
	{
		float greenToWhite = smoothstep(blueLimit, greenLimit, dist);
		color.rgb = lerp(GREEN, WHITE, greenToWhite);
	}

	/*float purpleToBlue = smoothstep(0.0f, 0.12f, dist);
	float blueToGreen = smoothstep(0.08f, 0.12f, dist);
	float greenToWhite = smoothstep(0.12f, 0.4f, dist);*/
	//float strength1 = 0;
	//color = lerp(purple, green, strength);
	/*color.rgb = lerp(PURPLE, BLUE, purpleToBlue);
	color.rgb = lerp(color, GREEN, blueToGreen);
	color.rgb = lerp(color, WHITE, greenToWhite);*/
	color.a = 1 - smoothstep(0.0f, 0.4f, dist);
}
