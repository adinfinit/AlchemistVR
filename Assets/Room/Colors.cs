﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Colors
{
	public static bool NeedUpdate (ref Color show, Color target)
	{
		if (Equal (show, target)) {
			return false;
		}

		show = Color.Lerp (show, target, 0.1f);
		return true;
	}

	public static bool Equal (Color a, Color b)
	{
		const float threshold = 0.01f;
		float d;

		d = a.r - b.r;
		if (d < -threshold || threshold < d) {
			return false;
		}

		d = a.g - b.g;
		if (d < -threshold || threshold < d) {
			return false;
		}

		d = a.b - b.b;
		if (d < -threshold || threshold < d) {
			return false;
		}

		return true;
	}
}

