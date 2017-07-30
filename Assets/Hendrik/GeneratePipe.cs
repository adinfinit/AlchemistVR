using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePipe : MonoBehaviour
{
	// prefab of a cylinder that is used to create the pipes
	public GameObject cylinder;

	// store the cylinder that the pipe is made out of
	GameObject cyl;

	// Generate a cylinder between points
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

	// generate the pipe between connection and return it
	public void Generate (World.Connection conn)
	{
		if (conn.source.tile.visual != null) {
			if (conn.drain.tile.visual != null) {

				GameObject t1 = (GameObject)conn.source.tile.visual;
				byte p1 = conn.sourcePort;
				GameObject t2 = (GameObject)conn.drain.tile.visual;
				byte p2 = conn.drainPort;

				float angle1 = Mathf.Deg2Rad * (p1 * 60f + 90f);
				float angle2 = Mathf.Deg2Rad * (p2 * 60f + 90f);

				Vector3 offset1 = new Vector3 (Mathf.Cos (angle1), 0, -Mathf.Sin (angle1)) * 2;
				Vector3 offset2 = new Vector3 (Mathf.Cos (angle2), 0, -Mathf.Sin (angle2)) * 2;

				Vector3 pos1 = (Vector3)t1.transform.position + (Vector3)(t1.transform.localToWorldMatrix * offset1);
				Vector3 pos2 = (Vector3)t2.transform.position + (Vector3)(t2.transform.localToWorldMatrix * offset2);

				GameObject connection = CylinderBetweenPoints (pos1, pos2);
				connection.name = conn.ToString ();

				cyl = connection;
			}
		}
	}
}
