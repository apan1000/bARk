//
//	Code repository for GPU noise development blog
Shader "Custom/Oak" 
{
	Properties 
	{
		_Octaves ("Octaves", Float) = 8.0
		_Frequency ("Frequency", Float) = 1.0
		_Amplitude ("Amplitude", Float) = 1.0
		_Lacunarity ("Lacunarity", Float) = 1.92
		_Persistence ("Persistence", Float) = 0.8
		_Offset ("Offset", Vector) = (0.0, 0.0, 0.0, 0.0)
		_CellType ("Cell Type", Float) = 1.0
		_DistanceFunction ("Distance Function", Float) = 1.0
		_Displacement("Displacement", Float) = 1.0		_LowColor("Low Color", Vector) = (0.0, 0.0, 0.0, 1.0)
		_HighColor("High Color", Vector) = (1.0, 1.0, 1.0, 1.0)

	}

	CGINCLUDE
		//
		//	FAST32_hash
		//	A very fast hashing function.  Requires 32bit support.
		//	http://briansharpe.wordpress.com/2011/11/15/a-fast-and-simple-32bit-floating-point-hash-function/
		//
		//	The hash formula takes the form....
		//	hash = mod( coord.x * coord.x * coord.y * coord.y, SOMELARGEFLOAT ) / SOMELARGEFLOAT
		//	We truncate and offset the domain to the most interesting part of the noise.
		//	SOMELARGEFLOAT should be in the range of 400.0->1000.0 and needs to be hand picked.  Only some give good results.
		//	3D Noise is achieved by offsetting the SOMELARGEFLOAT value by the Z coordinate
		//
		float4 FAST32_hash_3D_Cell( float3 gridcell )	//	generates 4 different random numbers for the single given cell point
		{
			//    gridcell is assumed to be an integer coordinate
		
			//	TODO: 	these constants need tweaked to find the best possible noise.
			//			probably requires some kind of brute force computational searching or something....
			const float2 OFFSET = float2( 50.0, 161.0 );
			const float DOMAIN = 69.0;
			const float4 SOMELARGEFLOATS = float4( 635.298681, 682.357502, 668.926525, 588.255119 );
			const float4 ZINC = float4( 48.500388, 65.294118, 63.934599, 63.279683 );
		
			//	truncate the domain
			gridcell.xyz = gridcell - floor(gridcell * ( 1.0 / DOMAIN )) * DOMAIN;
			gridcell.xy += OFFSET.xy;
			gridcell.xy *= gridcell.xy;
			return frac( ( gridcell.x * gridcell.y ) * ( 1.0 / ( SOMELARGEFLOATS + gridcell.zzzz * ZINC ) ) );
		}
		static const int MinVal = -1;
		static const int MaxVal = 1;
		float Cellular3D(float3 xyz, int cellType, int distanceFunction) 
		{
			int xi = int(floor(xyz.x));
			int yi = int(floor(xyz.y));
			int zi = int(floor(xyz.z));
		 
			float xf = xyz.x - float(xi);
			float yf = xyz.y - float(yi);
			float zf = xyz.z - float(zi);
		 
			float dist1 = 9999999.0;
			float dist2 = 9999999.0;
			float dist3 = 9999999.0;
			float dist4 = 9999999.0;
			float3 cell;
		 
			for (int z = MinVal; z <= MaxVal; z++) {
				for (int y = MinVal; y <= MaxVal; y++) {
					for (int x = MinVal; x <= MaxVal; x++) {
						cell = FAST32_hash_3D_Cell(float3(xi + x, yi + y, zi + z)).xyz;
						cell.x += (float(x) - xf);
						cell.y += (float(y) - yf);
						cell.z += (float(z) - zf);
						float dist = 0.0;
						if(distanceFunction <= 1)
						{
							dist = sqrt(dot(cell, cell));
						}
						else if(distanceFunction > 1 && distanceFunction <= 2)
						{
							dist = dot(cell, cell);
						}
						else if(distanceFunction > 2 && distanceFunction <= 3)
						{
							dist = abs(cell.x) + abs(cell.y) + abs(cell.z);
							dist *= dist;
						}
						else if(distanceFunction > 3 && distanceFunction <= 4)
						{
							dist = max(abs(cell.x), max(abs(cell.y), abs(cell.z)));
							dist *= dist;
						}
						else if(distanceFunction > 4 && distanceFunction <= 5)
						{
							dist = dot(cell, cell) + cell.x*cell.y + cell.x*cell.z + cell.y*cell.z;	
						}
						else if(distanceFunction > 5 && distanceFunction <= 6)
						{
							dist = pow(abs(cell.x*cell.x*cell.x*cell.x + cell.y*cell.y*cell.y*cell.y + cell.z*cell.z*cell.z*cell.z), 0.25);
						}
						else if(distanceFunction > 6 && distanceFunction <= 7)
						{
							dist = sqrt(abs(cell.x)) + sqrt(abs(cell.y)) + sqrt(abs(cell.z));
							dist *= dist;
						}
						if (dist < dist1) 
						{
							dist4 = dist3;
							dist3 = dist2;
							dist2 = dist1;
							dist1 = dist;
						}
						else if (dist < dist2) 
						{
							dist4 = dist3;
							dist3 = dist2;
							dist2 = dist;
						}
						else if (dist < dist3) 
						{
							dist4 = dist3;
							dist3 = dist;
						}
						else if (dist < dist4) 
						{
							dist4 = dist;
						}
					}
				}
			}
		 
			if(cellType <= 1)	// F1
				return dist1;	//	scale return value from 0.0->1.333333 to 0.0->1.0  	(2/3)^2 * 3  == (12/9) == 1.333333
			else if(cellType > 1 && cellType <= 2)	// F2
				return dist2;
			else if(cellType > 2 && cellType <= 3)	// F3
				return dist3;
			else if(cellType > 3 && cellType <= 4)	// F4
				return dist4;
			else if(cellType > 4 && cellType <= 5)	// F2 - F1 
				return dist2 - dist1;
			else if(cellType > 5 && cellType <= 6)	// F3 - F2 
				return dist3 - dist2;
			else if(cellType > 6 && cellType <= 7)	// F1 + F2/2
				return dist1 + dist2/2.0;
			else if(cellType > 7 && cellType <= 8)	// F1 * F2
				return dist1 * dist2;
			else if(cellType > 8 && cellType <= 9)	// Crackle
				return max(1.0, 10*(dist2 - dist1));
			else
				return dist1;
		}
		float CellNormal(float3 p, int octaves, float3 offset, float frequency, float amplitude, float lacunarity, float persistence, int cellType, int distanceFunction)
		{
			float sum = 0;
			for (int i = 0; i < octaves; i++)
			{
				float h = 0;
				h = Cellular3D((p+offset) * frequency, cellType, distanceFunction);
				sum += h*amplitude;
				frequency *= lacunarity;
				amplitude *= persistence;
			}
			return sum;
		}
	ENDCG

	SubShader 
	{
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma glsl
		#pragma target 3.0
		
		fixed _Octaves;
		float _Frequency;
		float _Amplitude;
		float3 _Offset;
		float _Lacunarity;
		float _Persistence;
		fixed _CellType;
		fixed _DistanceFunction;
		float _Displacement;
		fixed4 _LowColor;
		fixed4 _HighColor;

		struct Input 
		{
			float3 pos;
			float3 worldPos;
		};

		void vert (inout appdata_full v, out Input OUT)
		{
			UNITY_INITIALIZE_OUTPUT(Input, OUT);
			OUT.pos = v.vertex.xyz;

			float3 samplePos = float3(OUT.pos.x, OUT.pos.y, OUT.pos.z);	// Should be changed to match pixel-shader
			float h = CellNormal(samplePos, _Octaves, _Offset, _Frequency, _Amplitude, _Lacunarity, _Persistence, _CellType, _DistanceFunction);
			//v.vertex.xyz += v.normal * h * _Displacement;
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			float3 samplePos = float3(IN.worldPos.x * 3, IN.worldPos.y, IN.worldPos.z * 3);
			//float h = CellNormal(samplePos, _Octaves, _Offset, _Frequency, _Amplitude, _Lacunarity, _Persistence, _CellType, _DistanceFunction);
			float h = CellNormal(samplePos, 6, _Offset, 0.05f, 1.1f, 1.01f, 0.78f, 0, 0);

			h = h * 0.5 + 0.5;
			
			float4 color;
			color = lerp(_LowColor, fixed4(0.31f, 0.24, 0.2f, 1), h);

			o.Albedo = color.rgb;
			o.Alpha = 1.0;
		}
		ENDCG
	}
	
	FallBack "Diffuse"
}