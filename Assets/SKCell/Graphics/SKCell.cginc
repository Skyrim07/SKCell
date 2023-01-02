float N11(float x)
{
	return frac(sin(x * 236.98) * 192.211);
}
float N21(float2 xy)
{
	return frac(sin(xy.x * 789.95) * 123.36 * xy.y);
}
float2 N22(float2 p)
{
	float3 a = frac(p.xyx * float3(123.34, 234.34, 345.65));
	a += dot(a, a + 34.45);
	return frac(float2(a.x * a.y, a.y * a.z));
}

float Remap01(float x, float a, float b)
{
	return (x - a) / (b - a);
}

float Remap(float x, float a, float b, float c, float d)
{
	return c + Remap01(x, a, b) * (d - c);
}
float Band(float t, float start, float end, float w)
{
	float s1 = smoothstep(start - w, start + w, t);
	float s2 = smoothstep(end + w, end - w, t);
	return s1 * s2;
}

float Rect(float2 uv, float left, float right, float bottom, float up, float blur)
{
	float band1 = Band(uv.x, left, right, blur);
	float band2 = Band(uv.y, bottom, up, blur);
	return band1 * band2;
}
float4 RectBox(float2 uv, float wb, float wt, float yb, float yt, float blur)
{
	float c = 0.;
	c = smoothstep(yb - blur, yb + blur, uv.y);
	c *= smoothstep(yt + blur, yt - blur, uv.y);
    
	float w = Remap(uv.y, yb, yt, wb, wt);
	uv.x = abs(uv.x);
	c *= smoothstep(w + blur, w - blur, uv.x);

	return float4(c);
}

float Circle(float2 uv, float2 pos, float r, float w)
{
	float l = length(uv - pos);
	float s = smoothstep(r, r - w, l);
	return s;
}

// [2D] Return the distance from point p to line a,b
float DistLine(float2 p, float2 a, float2 b)
{
	float2 pa = p - a;
	float2 ba = b - a;
	float t = clamp(dot(pa, ba) / dot(ba, ba), 0., 1.);
	return length(pa - ba * t);
}

// [2D] Draw a line from point a to b
float Line(float2 p, float2 a, float2 b)
{
	float d = DistLine(p, a, b);
	float m = smoothstep(.012, .009, d);
	float l = length(a - b);
	m *= smoothstep(1.5, .9, length(a - b)) * .5 + smoothstep(.05, .03, abs(l - .75));
	return m;
}
// [2D] Get a rotation matrix with angle a
float2x2 Rot2D(float a)
{
	float s = sin(a), c = cos(a);
	return float2x2(c, -s, s, c);
}
float Voronoi(float2 uv, float time)
{
	float2 gv = frac(uv) - 0.5;
	float2 id = floor(uv);

	float minDist = 100;
	float2 cellid = float2(0, 0);
	for (float x = -1; x <= 1; x++)
		for (float y = -1; y <= 1; y++)
		{
			float2 offset = float2(x, y);
			float2 n = N22(offset + id);

			float2 p = offset + sin(n * time) * 0.5;
			float d = length(gv - p);
			if (d < minDist)
			{
				minDist = d;
				cellid = offset + id;
			}
		}
	float c = (cellid.x * 5 + cellid.y * 5 + 10) / 10;

	return c;
}