using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generators : MonoBehaviour
{

	public GameObject TilePrefab;
	public GameObject PipePrefab;
	float WallRadius = 10.25f;

	public GameObject newTileGameObject (World.Tile tile)
	{
		if (tile != null) {
			GameObject newTile = Instantiate (TilePrefab);

			if (tile.joints.Length == 1) {
				newTile.GetComponent<GenerateTile> ().Generate (tile.joints [0].ports);
				newTile.GetComponent<GenerateTile> ().tile = tile;
				newTile.name = tile.ToStringWithPorts ();

				tile.visual = newTile;
			} else if (tile.joints.Length == 2) {
				newTile.GetComponent<GenerateTile> ().Generate (tile.joints [0].ports, tile.joints [1].ports);
				newTile.GetComponent<GenerateTile> ().tile = tile;
				newTile.name = tile.ToStringWithPorts ();

				tile.visual = newTile;
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
