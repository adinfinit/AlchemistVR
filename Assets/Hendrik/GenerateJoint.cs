using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateJoint : MonoBehaviour
{

	public GameObject cylinder;
	public GameObject sphere;

	public GameObject[] spheres;
	public GameObject[] cylinders;

	GameObject CylinderBetweenPoints (Vector3 p1, Vector3 p2)
	{
		Vector3 offset = p2 - p1;
		Vector3 pos = p1 + (offset / 2);

		GameObject cyl = Instantiate (cylinder, Vector3.zero, Quaternion.identity);
		cyl.transform.parent = transform;
		cyl.transform.position = pos;

		Vector3 scale = new Vector3 (1, offset.magnitude / 2.0f, 1);
		cyl.transform.up = offset;
		cyl.transform.localScale = scale;

		return cyl;
	}

	public void Generate (byte[] connections, float heightDifference)
	{
		spheres = new GameObject[2];
		spheres [0] = GameObject.Instantiate (sphere);

		spheres [0].transform.parent = this.transform;

		spheres [0].transform.position = new Vector3 (0f, heightDifference, 0f);

		cylinders = new GameObject[connections.Length];

		for (int i = 0; i < connections.Length; i++) {
			// 0 is up
			int side = connections [i];

			Vector3 start = new Vector3 (0, heightDifference, 0);

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
