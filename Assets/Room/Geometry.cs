using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Geometry
{
	public static GameObject PrimitiveBetweenPoints (PrimitiveType primitive, Vector3 p1, Vector3 p2, float radius)
	{
		Vector3 offset = p2 - p1;
		Vector3 pos = p1 + (offset / 2);

		GameObject obj = GameObject.CreatePrimitive (primitive);
		obj.transform.position = pos;

		Vector3 scale = new Vector3 (radius, offset.magnitude / 2.0f, radius);
		obj.transform.up = offset;
		obj.transform.localScale = scale;

		return obj;
	}

	public static GameObject CylinderBetweenPoints (Vector3 p1, Vector3 p2, float radius)
	{
		return PrimitiveBetweenPoints (PrimitiveType.Cylinder, p1, p2, radius);
	}

	public static GameObject CapsuleBetweenPoints (Vector3 p1, Vector3 p2, float radius)
	{
		return PrimitiveBetweenPoints (PrimitiveType.Capsule, p1, p2, radius);
	}

	public static GameObject CubeBetweenPoints (Vector3 p1, Vector3 p2, float radius)
	{
		return PrimitiveBetweenPoints (PrimitiveType.Cube, p1, p2, radius);
	}

	public static GameObject Sphere (Vector3 p, float radius)
	{
		GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		sphere.transform.localScale = Vector3.one * radius;
		return sphere;
	}
}
