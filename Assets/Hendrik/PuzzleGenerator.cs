using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
	World.Wall puzzle;
	List<GameObject> tiles = new List<GameObject> ();
	List<GameObject> connPipes = new List<GameObject> ();

	float WallRadius = 10.25f;

	public GameObject TilePrefab;

	public GameObject cylinder;
	public GameObject sphere;

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

	// Use this for initialization
	void Start ()
	{
		puzzle = new World.Wall (null, 5, 10, 5, 5);
		World.Randomize.Wall (puzzle);

		for (int i = 0; i < puzzle.layers.Length; i++) {
			World.Layer layer = puzzle.layers [i];
			foreach (World.Tile tile in layer.tiles) {
				if (tile == null) {
					continue;
				}

				if (tile.joints.Length == 1) {
					GameObject newTile = (GameObject)Instantiate (TilePrefab);
					newTile.name = tile.ToStringWithPorts ();
					tiles.Add (newTile);

					GenerateTile genTile = newTile.GetComponent<GenerateTile> ();
					genTile.Generate (tile.joints [0].ports);

					tile.visual = newTile;

					TileData tileData = newTile.GetComponent<TileData> ();
					tileData.wall = puzzle;
					tileData.index = tile.index;
					tileData.layer = tile.layer.index;

				} else if (tile.joints.Length == 2) {
					GameObject newTile = (GameObject)Instantiate (TilePrefab);
					newTile.name = tile.ToStringWithPorts ();
					tiles.Add (newTile);

					GenerateTile genTile = newTile.GetComponent<GenerateTile> ();
					genTile.Generate (tile.joints [0].ports, tile.joints [1].ports);

					tile.visual = newTile;

					TileData tileData = newTile.GetComponent<TileData> ();
					tileData.wall = puzzle;
					tileData.index = tile.index;
					tileData.layer = tile.layer.index;

				}
			}
		}

		foreach (GameObject tile in tiles) {
			TileData tileData = tile.GetComponent<TileData> ();

			World.Tile tile2 = tileData.Tile ();
			/*
			float angle = tile2.Angle ();
			//tile.transform.position = new Vector3 (angle * (5 - 2.236f), tile2.Y () * 4, 0);
			tile.transform.Rotate (90, Mathf.Rad2Deg * angle + 90, 0);

			

			tile.transform.position = position;
			*/
			float angle = (float)tile2.index * 2f * Mathf.PI / (float)tile2.layer.tiles.Length;
			float y = (float)tile2.layer.index + (tile2.IsOffset () ? 0.5f : 0.0f);

			Vector3 position = new Vector3 ();
			position.x = Mathf.Cos (angle) * WallRadius;
			position.z = -Mathf.Sin (angle) * WallRadius;
			position.y = -y * 4;

			tile.transform.position = position;
			tile.transform.Rotate (90, Mathf.Rad2Deg * angle + 90, 180);
		}

		foreach (World.Connection conn in puzzle.connections) {
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
					connPipes.Add (connection);
				}
			}
		}

	}

	// Update is called once per frame
	void Update ()
	{
		
	}
}
