int getNumOfDigits(in int level, out int4 digits)
{
	int numOfDigits = 0;
	uint dividedLevel = level;
	digits = int4(0, 0, 0, 0);
	int _digits[4] = { 0, 0, 0, 0 };

	// unroll maybe?
	for (uint i = 3; i >= 0; i--)
	{
		numOfDigits++;
		// last digit
		_digits[i] = dividedLevel % 10;
		dividedLevel /= 10;
		if (dividedLevel == 0)
			break;
	}

	digits.x = _digits[0];
	digits.y = _digits[1];
	digits.z = _digits[2];
	digits.w = _digits[3];
	return numOfDigits;
}

float3 getUV(in float2 baseUV, in int level)
{
	int4 digits;
	int numOfDigits = getNumOfDigits(level, digits);
	float2 uv = baseUV;

	float splitValue = 1.0f / numOfDigits;
	for (int i = 0; i < numOfDigits; i++)
	{
		if ((1 - uv.x) < splitValue * (i + 1))
		{
			uv.x *= numOfDigits;
			return float3(uv, digits[3 - i]);
		}
	}

	return float3(0, 0, 0);
}

void sampleNumber_float(in SamplerState ss, in Texture2DArray arr, in float2 baseUV, in int level, out float4 outColor)
{
	float3 uv_level = getUV(baseUV, level);
	outColor = SAMPLE_TEXTURE2D_ARRAY(arr, ss, uv_level.xy, (int)uv_level.z);
}