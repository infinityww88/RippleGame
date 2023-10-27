#ifndef MYFAC_INCLUDE
#define MYFAC_INCLUDE

void MyFac_float(float2 Vec, out float Fac) {
	float TAU = 6.28318530;
	float2 nv = normalize(Vec);
	float a = acos(nv);
	if (nv.y < 0) {
		a = TAU  - a;
	}
	Fac = a / TAU;
}
#endif