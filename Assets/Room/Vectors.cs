using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vectors
{

	public static bool Equal (Vector3 a, Vector3 b)
	{
		const float threshold = 0.01f;
		float d;

		d = a.x - b.x;
		if (d < -threshold || threshold < d) {
			return false;
		}
		d = a.y - b.y;
		if (d < -threshold || threshold < d) {
			return false;
		}
		d = a.z - b.z;
		if (d < -threshold || threshold < d) {
			return false;
		}

		return true;
	}
}

