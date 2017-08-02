using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Geometry
{
	public static GameObject CylinderBetweenPoints (Vector3 p1, Vector3 p2, float radius)
	{
		Vector3 offset = p2 - p1;
		Vector3 pos = p1 + (offset / 2);

		GameObject cylinder = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
		cylinder.transform.position = pos;

		Vector3 scale = new Vector3 (radius, offset.magnitude / 2.0f, radius);
		cylinder.transform.up = offset;
		cylinder.transform.localScale = scale;

		return cylinder;
	}

	public static GameObject CapsuleBetweenPoints (Vector3 p1, Vector3 p2, float radius)
	{
		Vector3 offset = p2 - p1;
		Vector3 pos = p1 + (offset / 2);

		GameObject cylinder = GameObject.CreatePrimitive (PrimitiveType.Capsule);
		cylinder.transform.position = pos;

		Vector3 scale = new Vector3 (radius, offset.magnitude / 2.0f, radius);
		cylinder.transform.up = offset;
		cylinder.transform.localScale = scale;

		return cylinder;
	}

	public static GameObject Sphere (Vector3 p, float radius)
	{
		GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		sphere.transform.localScale = Vector3.one * radius;
		return sphere;
	}
}
