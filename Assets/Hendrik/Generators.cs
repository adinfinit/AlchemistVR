using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generators : MonoBehaviour
{

	public GameObject TilePrefab;
	public GameObject PipePrefab;
	float WallRadius = 10.25f;

	public static GameObject CreateTile (World.Tile tile)
	{
		GameObject tileObj = new GameObject ();
		int index = 0;
		foreach (World.Joint joint in tile.joints) {
			GameObject jointObj = CreateJoint (joint, index);
			index++;
			jointObj.transform.parent = tileObj.transform;
		}
		tile.visual = tileObj;
		tileObj.name = tile.ToString ();

		GameObject box = GameObject.CreatePrimitive (PrimitiveType.Cube);
		box.transform.localScale = new Vector3 (1f, 1f, 0.1f);
		box.transform.SetParent (tileObj.transform, false);

		return tileObj;
	}

	public static GameObject CreateJoint (World.Joint joint, int index)
	{
		GameObject joinObj = new GameObject ();
		joinObj.name = joint.ToString ();
		joint.gameObject = joinObj;

		GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		sphere.transform.localScale = Vector3.one * 0.3f;
		sphere.transform.parent = joinObj.transform;

		sphere.transform.localPosition = new Vector3 (0f, 0f, (index & 1) == 1 ? -0.35f : 0.35f);
		foreach (byte port in joint.ports) {
			float angle = (float)port * Mathf.PI * 2f / 6f;

			Vector3 portPosition = Vector3.zero;
			portPosition.y = Mathf.Cos (angle);
			portPosition.x = -Mathf.Sin (angle);

			GameObject line = CylinderBetweenPoints (portPosition, sphere.transform.localPosition, 0.2f);
			line.name = "Port " + port;
			line.transform.SetParent (joinObj.transform, false);
		}

		return joinObj;
	}

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

	public GameObject newTileGameObject (World.Tile tile)
	{
		if (tile == null) {
			Debug.Break ();
		}
		if (tile != null) {
			GameObject newTile = Instantiate (TilePrefab);

			if (tile.joints.Length == 1) {
				newTile.GetComponent<GenerateTile> ().Generate (tile.joints [0].ports);
				newTile.GetComponent<GenerateTile> ().tile = tile;
				newTile.name = tile.ToStringWithPorts ();

				tile.visual = newTile;
				tile.joints [0].gameObject = newTile.GetComponent<GenerateTile> ().joints [0];
			} else if (tile.joints.Length == 2) {
				newTile.GetComponent<GenerateTile> ().Generate (tile.joints [0].ports, tile.joints [1].ports);
				newTile.GetComponent<GenerateTile> ().tile = tile;
				newTile.name = tile.ToStringWithPorts ();

				tile.visual = newTile;
				tile.joints [0].gameObject = newTile.GetComponent<GenerateTile> ().joints [0];
				tile.joints [1].gameObject = newTile.GetComponent<GenerateTile> ().joints [1];
			} 

			float angle = (float)tile.index * 2f * Mathf.PI / (float)tile.layer.tiles.Length;
			float y = (float)tile.layer.index + (tile.IsOffset () ? 0.5f : 0.0f);

			Vector3 position = new Vector3 ();
			position.x = Mathf.Cos (angle) * WallRadius;
			position.z = -Mathf.Sin (angle) * WallRadius;
			position.y = -y * 4;

			newTile.transform.position = position;
			newTile.transform.Rotate (90, Mathf.Rad2Deg * angle + 90, 180);

			return newTile;
		}
		return null;
	}

	public GameObject newConnectionGameObject (World.Connection conn)
	{
		GameObject newConn = Instantiate (PipePrefab);
		newConn.GetComponent<GeneratePipe> ().Generate (conn);

		return newConn;
	}
}
