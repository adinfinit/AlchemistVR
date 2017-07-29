using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTile : MonoBehaviour
{
	GameObject[] spheres;
	GameObject[] cylinders;

	// when two overlaping pipes are generated,
	// the middle of the pipe is this much above the middle
	float heightDifference = 0.6f;

	// Use this for initialization
	void Start ()
	{
	}

	void Generate (int[] connections)
	{
		// generate spheres
		spheres = new GameObject[1];
		spheres [0] = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		spheres [0].transform.parent = this.transform;
		spheres [0].transform.position = new Vector3 (0f, 0f, 0f);	

		cylinders = new GameObject[connections.GetLength (0)];

		// generate cylinders
		for (int i = 0; i < connections.GetLength (0); i++) {
			cylinders [i] = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
			cylinders [i].transform.parent = this.transform;
			// 0 is up
			int side = connections [i];

			float angle = Mathf.Deg2Rad * (side * 60f + 90f);

			float dx = Mathf.Cos (angle);
			float dz = -Mathf.Sin (angle);

			cylinders [i].transform.position = new Vector3 (dx, 0f, dz);

			cylinders [i].transform.Rotate (0f, Mathf.Rad2Deg * angle, 90f);

		}

	}

	void Generate (int[] connection1, int[] connection2)
	{
		spheres = new GameObject[2];
		spheres [0] = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		spheres [1] = GameObject.CreatePrimitive (PrimitiveType.Sphere);

		spheres [0].transform.parent = this.transform;
		spheres [1].transform.parent = this.transform;

		spheres [0].transform.position = new Vector3 (0f, heightDifference, 0f);
		spheres [1].transform.position = new Vector3 (0f, -heightDifference, 0f);

		cylinders = new GameObject[connection1.GetLength (0) + connection2.GetLength (0)];

		for (int i = 0; i < connection1.GetLength (0); i++) {
			cylinders [i] = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
			cylinders [i].transform.parent = this.transform;
			// 0 is up
			int side = connection1 [i];

			float angle = Mathf.Deg2Rad * (side * 60f + 90f);

			float angle2 = Mathf.Rad2Deg * Mathf.Atan (-heightDifference);

			float dx = Mathf.Cos (angle);
			float dz = -Mathf.Sin (angle);

			cylinders [i].transform.position = new Vector3 (dx, 0f, dz);

			cylinders [i].transform.Rotate (0, Mathf.Rad2Deg * angle, 90f + angle2);

		}

		for (int i = 0; i < connection2.GetLength (0); i++) {
			cylinders [i] = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
			cylinders [i].transform.parent = this.transform;
			// 0 is up
			int side = connection2 [i];

			float angle = Mathf.Deg2Rad * (side * 60f + 90f);

			float angle2 = Mathf.Rad2Deg * Mathf.Atan (heightDifference);

			float dx = Mathf.Cos (angle);
			float dz = -Mathf.Sin (angle);

			cylinders [1].transform.localScale = new Vector3 (1, 1 + heightDifference / 2, 1);
			cylinders [i].transform.position = new Vector3 (dx, 0f, dz);

			cylinders [i].transform.Rotate (0, Mathf.Rad2Deg * angle, 90f + angle2);

		}
	}
}
