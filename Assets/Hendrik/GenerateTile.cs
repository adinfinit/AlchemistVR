using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTile : MonoBehaviour
{
	GameObject[] spheres;
	GameObject[] cylinders;
	public GameObject cylinder;
	public GameObject sphere;

	// when two overlaping pipes are generated,
	// the middle of the pipe is this much above the middle
	float heightDifference = 0.6f;

	GameObject CylinderBetweenPoints (Vector3 p1, Vector3 p2)
	{
		Vector3 offset = p2 - p1;
		Vector3 pos = p1 + (offset / 2);

		GameObject cyl = Instantiate (cylinder, Vector3.zero, Quaternion.identity);
		cyl.transform.position = pos;

		Vector3 scale = new Vector3 (1, offset.magnitude / 2.0f, 1);
		cyl.transform.up = offset;
		cyl.transform.localScale = scale;

		return cyl;
	}

	public void Generate (byte[] connections)
	{
		// generate spheres
		spheres = new GameObject[1];
		spheres [0] = GameObject.Instantiate (sphere);
		spheres [0].transform.parent = this.transform;
		spheres [0].transform.position = new Vector3 (0f, 0f, 0f);	

		cylinders = new GameObject[connections.GetLength (0)];

		// generate cylinders
		for (int i = 0; i < connections.GetLength (0); i++) {
			cylinders [i] = Instantiate (cylinder);
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

	public void Generate (byte[] connection1, byte[] connection2)
	{
		spheres = new GameObject[2];
		spheres [0] = GameObject.Instantiate (sphere);
		spheres [1] = GameObject.Instantiate (sphere);

		spheres [0].transform.parent = this.transform;
		spheres [1].transform.parent = this.transform;

		spheres [0].transform.position = new Vector3 (0f, heightDifference, 0f);
		spheres [1].transform.position = new Vector3 (0f, -heightDifference, 0f);

		cylinders = new GameObject[connection1.GetLength (0) + connection2.GetLength (0)];

		for (int i = 0; i < connection1.GetLength (0); i++) {
			// 0 is up
			int side = connection1 [i];

			Vector3 start = new Vector3 (0, heightDifference, 0);

			float angle = Mathf.Deg2Rad * (side * 60f + 90f);
			float dx = Mathf.Cos (angle);
			float dz = -Mathf.Sin (angle);

			Vector3 end = new Vector3 (dx, 0, dz);
			end.Scale (new Vector3 (2, 2, 2));

			cylinders [i] = CylinderBetweenPoints (start, end);
			cylinders [i].transform.parent = this.transform;
		}

		for (int i = 0; i < connection2.GetLength (0); i++) {
			// 0 is up
			int side = connection2 [i];

			Vector3 start = new Vector3 (0, -heightDifference, 0);

			float angle = Mathf.Deg2Rad * (side * 60f + 90f);
			float dx = Mathf.Cos (angle);
			float dz = -Mathf.Sin (angle);

			Vector3 end = new Vector3 (dx, 0, dz);
			end.Scale (new Vector3 (2, 2, 2));

			cylinders [i] = CylinderBetweenPoints (start, end);
			cylinders [i].transform.parent = this.transform;
		}
	}
}
