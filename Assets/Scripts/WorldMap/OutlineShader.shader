// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ThickLine"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_Thickness("Thickness", float) = 4
	}
		SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geo

			#include "UnityCG.cginc"

			struct v2g
			{
				float4 vertex : SV_POSITION;
			};

			struct g2f
			{
				float4 pos: POSITION;
			};

			fixed4 _Color;
			float _Thickness;

			v2g vert(float4 position : POSITION)
			{
				v2g o;
				o.vertex = position;
				return o;
			}

			[maxvertexcount(4)]
			void geo(line v2g v[2], inout TriangleStream<g2f> ts)
			{
				g2f g[4];

				float4 p1 = UnityObjectToClipPos(v[0].vertex);
				float4 p2 = UnityObjectToClipPos(v[1].vertex);

				float2 dir = normalize(p2.xy - p1.xy);
				float2 normal = float2(-dir.y, dir.x);

				float4 offset1 = float4(normal * p1.w * _Thickness / 2.0f, 0, 0);
				float4 offset2 = float4(normal * p2.w * _Thickness / 2.0f, 0, 0);

				float4 o1 = p1 + offset1;
				float4 o2 = p1 - offset1;
				float4 o3 = p2 + offset2;
				float4 o4 = p2 - offset2;

				g[0].pos = o1;
				g[1].pos = o2;
				g[2].pos = o3;
				g[3].pos = o4;

				ts.Append(g[0]);
				ts.Append(g[1]);
				ts.Append(g[2]);
				ts.Append(g[3]);
			}

			fixed4 frag(g2f i) : SV_Target
			{
				return _Color;
			}
			ENDCG
		}
	}
}